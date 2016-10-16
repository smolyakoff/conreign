using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    public class BusGrain : Grain<BusState>
    {
        private static Dictionary<Type, MethodInfo> _handlerMethodCache;
        private IAsyncStream<IServerEvent> _stream;
        private Dictionary<Guid, StreamSubscriptionHandle<IServerEvent>> _subscriptions;

        public override async Task OnActivateAsync()
        {
            await InitializeState();
            _handlerMethodCache = new Dictionary<Type, MethodInfo>();
            var provider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            var stream = provider.GetStream<IServerEvent>(State.StreamId, State.Topic);
            _stream = stream;
            await RestoreSubscriptions();
            await base.OnActivateAsync();
        }

        public async Task Subscribe(IEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (State.HandlerSubscriptions.ContainsKey(handler))
            {
                return;
            }
            var subscription = await SubscribeInternal(handler);
            if (subscription == null)
            {
                return;
            }
            _subscriptions[subscription.HandleId] = subscription;
            State.HandlerSubscriptions[handler] = subscription.HandleId;
            await WriteStateAsync();
        }

        public async Task Unsubscribe(IEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (!State.HandlerSubscriptions.ContainsKey(handler))
            {
                return;
            }
            var id = State.HandlerSubscriptions[handler];
            var subscription = _subscriptions[id];
            State.HandlerSubscriptions.Remove(handler);
            _subscriptions.Remove(id);
            await subscription.UnsubscribeAsync();
            await WriteStateAsync();
        }

        public async Task Notify(params IServerEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            //var ts = TaskScheduler.Current;
            //foreach (var @event in events)
            //{
            //    Console.WriteLine("Next Start");
            //    await _stream.OnNextAsync(@event);
            //    Console.WriteLine("Next End");
            //}

        }

        private async Task<StreamSubscriptionHandle<IServerEvent>> ResumeInternal(IEventHandler handler, StreamSubscriptionHandle<IServerEvent> subscription)
        {
            var func = CreateHandlerFunctionOrNull(handler);
            if (func == null)
            {
                return null;
            }
            await subscription.ResumeAsync(func);
            return subscription;
        }

        private async Task<StreamSubscriptionHandle<IServerEvent>> SubscribeInternal(IEventHandler handler)
        {
            var func = CreateHandlerFunctionOrNull(handler);
            if (func == null)
            {
                return null;
            }
            var subscription = await _stream.SubscribeAsync(func);
            return subscription;
        }

        private bool Test(IStreamIdentity stream, object filterdata, object item)
        {
            throw new NotImplementedException();
        }

        private List<Type> GetSupportedEvents(IEventHandler handler)
        {
            var eventTypes = handler
                .GetType()
                .GetInterfaces()
                .Where(x => typeof(IEventHandler).IsAssignableFrom(x) && x.IsGenericType)
                .Select(x => x.GetGenericArguments()[0])
                .Where(x => typeof(IServerEvent).IsAssignableFrom(x))
                .ToList();
            foreach (var eventType in eventTypes.Where(x => !_handlerMethodCache.ContainsKey(x)))
            {
                var type = typeof(IEventHandler<>).MakeGenericType(eventType);
                var method = type.GetMethod("Handle");
                _handlerMethodCache.Add(eventType, method);
            }
            return eventTypes;
        }

        private Func<IServerEvent, StreamSequenceToken, Task> CreateHandlerFunctionOrNull(IEventHandler handler)
        {
            var supportedTypes = GetSupportedEvents(handler);
            if (supportedTypes.Count == 0)
            {
                return null;
            }
            return async (@event, token) =>
            {
                var targetTypes = supportedTypes.Where(t => t.IsInstanceOfType(@event));
                foreach (var targetType in targetTypes)
                {
                    await Handle(targetType, handler, @event);
                }
            };
        }

        private static Task Handle(Type baseType, IEventHandler handler, IServerEvent @event)
        {
            var method = _handlerMethodCache[baseType];
            return (Task) method.Invoke(handler, new object[]{@event});
        }

        private async Task InitializeState()
        {
            State.Topic = this.GetPrimaryKeyString();
            if (State.StreamId == Guid.Empty)
            {
                State.StreamId = Guid.NewGuid();
                await WriteStateAsync();
            }
        }

        private async Task RestoreSubscriptions()
        {
            var handles = await _stream.GetAllSubscriptionHandles();
            _subscriptions = handles.ToDictionary(x => x.HandleId, x => x);
            var tasks = new List<Task>();
            foreach (var pair in State.HandlerSubscriptions)
            {
                StreamSubscriptionHandle<IServerEvent> subscription;
                var hasSubscription = _subscriptions.TryGetValue(pair.Value, out subscription);
                tasks.Add(hasSubscription ? ResumeInternal(pair.Key, subscription) : SubscribeInternal(pair.Key));
            }
            var toUnsubscribe = _subscriptions
                .Where(x => !State.HandlerSubscriptions.Values.Contains(x.Key))
                .Select(pair => pair.Value.UnsubscribeAsync());
            tasks.AddRange(toUnsubscribe);
            await Task.WhenAll(tasks);
        }
    }
}
