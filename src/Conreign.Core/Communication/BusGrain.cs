using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    public class BusGrain : Grain<BusState>, IBusGrain
    {
        private Dictionary<Type, MethodInfo> _handlerMethodCache;
        private IAsyncStream<ISystemEvent> _stream;
        private StreamSubscriptionHandle<ISystemEvent> _subscription;
        private Logger _logger;

        public override async Task OnActivateAsync()
        {
            await InitializeState();
            _handlerMethodCache = new Dictionary<Type, MethodInfo>();
            _logger = GetLogger(typeof(BusGrain).Name);
            var provider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            var stream = provider.GetStream<ISystemEvent>(State.StreamId, State.Topic);
            _stream = stream;
            var handles = await stream.GetAllSubscriptionHandles();
            if (handles.Count > 0)
            {
                _subscription = handles[0];
                await _subscription.ResumeAsync(OnNext, OnError, OnCompleted, State.StreamSequenceToken);
            }
            else
            {
                _subscription = await stream.SubscribeAsync(OnNext, OnError, OnCompleted, State.StreamSequenceToken);
            }
            await base.OnActivateAsync();
        }

        public async Task Subscribe(Type baseType, IEventHandler handler)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (!typeof(ISystemEvent).IsAssignableFrom(baseType))
            {
                throw new ArgumentException("Base type should implement ISystemEvent interface.", nameof(baseType));
            }
            var @interface = handler
                .GetType()
                .GetInterfaces()
                .Where(x => typeof(IEventHandler).IsAssignableFrom(x) && x.IsGenericType)
                .FirstOrDefault(x => baseType.IsAssignableFrom(x.GetGenericArguments()[0]));
            if (@interface == null)
            {
                throw new ArgumentException($"Expected interface to be IEventHandler<${baseType.Name}>.");
            }
            var set = State.Subscribers.ContainsKey(baseType)
                ? State.Subscribers[baseType]
                : new HashSet<IEventHandler>();
            State.Subscribers[baseType] = set;
            set.Add(handler);

            if (!_handlerMethodCache.ContainsKey(baseType))
            {
                var type = typeof(IEventHandler<>).MakeGenericType(baseType);
                var method = type.GetMethod("Handle");
                _handlerMethodCache.Add(baseType, method);
            }
            await WriteStateAsync();
        }

        public async Task Unsubscribe(Type baseType, IEventHandler handler)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (!typeof(ISystemEvent).IsAssignableFrom(baseType))
            {
                throw new ArgumentException("Base type should implement ISystemEvent interface", nameof(baseType));
            }
            if (!State.Subscribers.ContainsKey(baseType))
            {
                return;
            }
            var set = State.Subscribers[baseType];
            var removed = set.Remove(handler);
            if (removed)
            {
                await WriteStateAsync();
            }
        }

        public async Task UnsubscribeAll(IEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            var removed = false;
            foreach (var set in State.Subscribers.Values)
            {
                removed = set.Remove(handler);
            }
            if (removed)
            {
                await WriteStateAsync();
            } 
        }

        public async Task Notify(params ISystemEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            var ts = TaskScheduler.Current;
            await Task.Factory.StartNew(async () =>
            {
                foreach (var @event in events)
                {
                    await _stream.OnNextAsync(@event);
                }
            }, CancellationToken.None, TaskCreationOptions.None, ts);
        }

        private async Task OnNext(ISystemEvent @event, StreamSequenceToken token)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var type = @event.GetType();
            var subscribers = State.Subscribers
                .Where(pair => pair.Key.IsAssignableFrom(type))
                .SelectMany(pair => pair.Value.Select(handler => new
                {
                    BaseType = pair.Key,
                    Handler = handler
                }))
                .ToList();
            try
            {
                var ts = TaskScheduler.Current;
                await Task.Factory.StartNew(() =>
                {
                    var tasks = subscribers
                        .Select(x => Handle(x.BaseType, x.Handler, @event))
                        .ToList();
                    return Task.WhenAll(tasks);
                }, CancellationToken.None, TaskCreationOptions.None, ts);
            }
            finally
            {
                State.StreamSequenceToken = token;
            }
            await WriteStateAsync();
        }

        private Task Handle(Type baseType, IEventHandler handler, ISystemEvent @event)
        {
            var method = _handlerMethodCache[baseType];
            return (Task) method.Invoke(handler, new object[]{@event});
        }

        private Task OnCompleted()
        {
            _logger.Error(
                (int) CommunicationError.BusStreamUnexpectedlyCompleted,
                $"Bus stream [{State.Topic}:{State.StreamId}] unexpectedly completed.");
            DeactivateOnIdle();
            return Task.CompletedTask;
        }

        private Task OnError(Exception ex)
        {
            _logger.Error(
                (int) CommunicationError.BusStreamUnexpectedException,
                $"Bus stream [{State.Topic}:{State.StreamId}] unexpected exception: {ex.Message}",
                ex);
            return Task.CompletedTask;
        }

        private async Task InitializeState()
        {
            State.Topic = this.GetPrimaryKeyString();
            if (State.StreamId == Guid.Empty)
            {
                State.StreamId = Guid.NewGuid();
            }
            await WriteStateAsync(); 
        }
    }
}
