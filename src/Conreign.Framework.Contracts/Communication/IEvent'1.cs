using System.Collections.Immutable;
using MediatR;

namespace Conreign.Framework.Contracts.Communication
{
    public interface IEvent<out T> : IAsyncNotification
    {
        T Payload { get; }

        IImmutableSet<string> ConnectionIds { get; }
    }
}