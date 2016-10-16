using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    public class Publisher<T> : IPublisher<T>, IEquatable<Publisher<T>> where T : class, IEvent
    {
        public bool Equals(Publisher<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _stream.Equals(other._stream);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Publisher<T>) obj);
        }

        public override int GetHashCode()
        {
            return _stream.GetHashCode();
        }

        public static bool operator ==(Publisher<T> left, Publisher<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Publisher<T> left, Publisher<T> right)
        {
            return !Equals(left, right);
        }

        private readonly IAsyncStream<T> _stream;

        public Publisher(IAsyncStream<T> stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            _stream = stream;
        }

        public async Task Notify(params T[] events)
        {
            if (events.Any(e => e.GetType() == typeof(GameStarted.Server)))
            {
                var x = 1;
            }
            foreach (var @event in events)
            {
                await _stream.OnNextAsync(@event);
            }
        }
    }
}