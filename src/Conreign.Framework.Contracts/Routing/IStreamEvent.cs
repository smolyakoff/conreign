using MediatR;

namespace Conreign.Framework.Contracts.Routing
{
    public interface IStreamEvent : IAsyncNotification
    {
        StreamKey StreamKey { get; }
    }
}