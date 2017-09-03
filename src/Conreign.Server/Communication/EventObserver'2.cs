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
    internal class EventObserver<T, TEvent> : IAsyncObserver<TEvent> where T : Grain, IEventHandler
        where TEvent : IEvent
    {
        private readonly T _grain;
        private readonly List<Type> _types;

        public EventObserver(T grain, List<Type> types)
        {
            _grain = grain;
            _types = types;
        }

        public Task OnNextAsync(TEvent item, StreamSequenceToken token = null)
        {
            if (!_types.Any(x => x.IsInstanceOfType(item)))
            {
                return TaskCompleted.Completed;
            }
            dynamic h = _grain;
            return (Task) h.Handle((dynamic) item);
        }

        public Task OnCompletedAsync()
        {
            return TaskCompleted.Completed;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return TaskCompleted.Completed;
        }
    }
}