using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Server.Contracts.Communication;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Communication
{
    public static class StreamExtensions
    {
        public static async Task<StreamSubscriptionHandle<TEvent>> EnsureIsSubscribedOnce<T, TEvent>(this T handler,
            IAsyncStream<TEvent> stream)
            where T : Grain, IEventHandler
            where TEvent : IEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            var eventTypes = GetSupportedEventTypes(handler);
            if (eventTypes.Count == 0)
            {
                throw new ArgumentException($"No events could be handled by {handler.GetType().Name}.");
            }
            var handles = await stream.GetAllSubscriptionHandles();
            var observer = new EventObserver<T, TEvent>(handler, eventTypes);
            if (handles.Count > 0)
            {
                for (var i = 0; i < handles.Count; i++)
                {
                    if (i == handles.Count - 1)
                    {
                        await handles[i].ResumeAsync(observer);
                        return handles[i];
                    }
                    await handles[i].UnsubscribeAsync();
                }
            }
            var handle = await stream.SubscribeAsync(observer);
            return handle;
        }

        public static IAsyncStream<IServerEvent> GetServerStream(this IStreamProvider provider, string topic)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.GetStream<IServerEvent>(Guid.Empty, topic);
        }

        private static List<Type> GetSupportedEventTypes<T>(T handler) where T : Grain, IEventHandler
        {
            return handler
                .GetType()
                .GetInterfaces()
                .Where(x => typeof(IEventHandler).IsAssignableFrom(x) && x.IsGenericType)
                .Select(x => x.GetGenericArguments()[0])
                .Where(x => typeof(IEvent).IsAssignableFrom(x))
                .ToList();
        }
    }
}