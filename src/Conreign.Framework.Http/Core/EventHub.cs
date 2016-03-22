using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Routing;
using Conreign.Framework.Http.Core.Data;
using MediatR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Streams;
using Serilog;

namespace Conreign.Framework.Http.Core
{
    public class EventHub : Hub<IEventHubClient>
    {
        private readonly Task<StreamSubscriptionHandle<IRoutedEventEnvelope<object>>> _initializationTask;
        private readonly IMediator _mediator;

        public EventHub(IMediator mediator, EventHubSettings settings)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            settings.EnsureIsValid();
            _mediator = mediator;
            var provider = GrainClient.GetStreamProvider(settings.StreamProviderName);
            var stream = provider.GetStream<IRoutedEventEnvelope<object>>(settings.StreamKey.Id,
                settings.StreamKey.Namespace);
            _initializationTask = stream.SubscribeAsync(new EventHubObserver(this));
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();
            var action = new HttpAction {Type = "connect", Meta = GetMeta()};
            await _mediator.SendAsync(action);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            var action = new HttpAction {Type = "disconnect", Meta = GetMeta()};
            await _mediator.SendAsync(action);
        }

        protected override void Dispose(bool disposing)
        {
            _initializationTask.Wait(3000);
            _initializationTask.Result.UnsubscribeAsync().Wait(3000);
            base.Dispose(disposing);
        }

        private JObject GetMeta()
        {
            var token = Context.Headers["Authorization"]?.Replace("Bearer", string.Empty)?.Trim();
            var meta = JObject.FromObject(new {auth = new {accessToken = token}});
            return meta;
        }

        private class EventHubObserver : IAsyncObserver<IRoutedEventEnvelope<object>>
        {
            private readonly EventHub _hub;
            private readonly ILogger _logger;

            public EventHubObserver(EventHub hub)
            {
                _logger = Log.ForContext(GetType());
                _hub = hub;
            }

            public Task OnNextAsync(IRoutedEventEnvelope<object> @event, StreamSequenceToken token = null)
            {
                var client = @event.ConnectionIds == null
                    ? _hub.Clients.All
                    : _hub.Clients.Clients(@event.ConnectionIds.ToList());
                client.Receive(@event);
                return TaskDone.Done;
            }

            public Task OnCompletedAsync()
            {
                _logger.Fatal("Global event stream unexpectedly completed.");
                return TaskDone.Done;
            }

            public Task OnErrorAsync(Exception ex)
            {
                _logger.Error(ex, "Event stream error: {Message}", ex.Message);
                return TaskDone.Done;
            }
        }
    }
}