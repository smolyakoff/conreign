using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Api.Framework;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Game.Actions;
using MediatR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Streams;
using Serilog;

namespace Conreign.Api
{
    public class EventHub : Hub<IClient>
    {
        private readonly IMediator _mediator;
        private readonly Task<StreamSubscriptionHandle<EventEnvelope<object>>> _initializationTask;

        public EventHub(IMediator mediator)
        {
            _mediator = mediator;
            var provider = GrainClient.GetStreamProvider("DefaultStream");
            var stream = provider.GetStream<EventEnvelope<object>>(Guid.Empty, null);
            _initializationTask = stream.SubscribeAsync(new EventHubObserver(this));
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();
            var action = new GenericAction {Type = "connect", Meta = GetMeta()};
            await _mediator.SendAsync(action);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            var action = new GenericAction { Type = "disconnect", Meta = GetMeta() };
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
            var meta = JObject.FromObject(new { auth = new { accessToken = token }});
            return meta;
        }

        private class EventHubObserver: IAsyncObserver<EventEnvelope<object>>
        {
            private readonly ILogger _logger;

            private readonly EventHub _hub;

            public EventHubObserver(EventHub hub)
            {
                _logger = Log.ForContext(GetType());
                _hub = hub;
            }

            public Task OnNextAsync(EventEnvelope<object> envelope, StreamSequenceToken token = null)
            {
                var client = envelope.ConnectionIds == null 
                    ? _hub.Clients.All 
                    : _hub.Clients.Clients(envelope.ConnectionIds.ToList());
                client.HandleEvent(envelope.Event);
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

    public interface IClient
    {
        void HandleEvent(object @event);
    }
}
