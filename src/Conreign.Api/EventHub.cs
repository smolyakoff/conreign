using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Orleans;
using Orleans.Streams;

namespace Conreign.Api
{
    public class EventHub : Hub<IClient>
    {
        private readonly Task<StreamSubscriptionHandle<object>> _initializationTask; 

        public EventHub()
        {
            var provider = GrainClient.GetStreamProvider("DefaultStream");
            var stream = provider.GetStream<object>(Guid.Empty, null);
            _initializationTask = stream.SubscribeAsync(new Observer(this));
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        protected override void Dispose(bool disposing)
        {
            _initializationTask.Wait(3000);
            _initializationTask.Result.UnsubscribeAsync().Wait(3000);
            base.Dispose(disposing);
        }

        private class Observer: IAsyncObserver<object>
        {
            private readonly EventHub _hub;

            public Observer(EventHub hub)
            {
                _hub = hub;
            }

            public Task OnNextAsync(object item, StreamSequenceToken token = null)
            {
                _hub.Clients.All.HandleEvent(item);
                return TaskDone.Done;
            }

            public Task OnCompletedAsync()
            {
                throw new NotImplementedException();
            }

            public Task OnErrorAsync(Exception ex)
            {
                throw new NotImplementedException();
            }
        }

    }

    public interface IClient
    {
        void HandleEvent(object @event);
    }
}
