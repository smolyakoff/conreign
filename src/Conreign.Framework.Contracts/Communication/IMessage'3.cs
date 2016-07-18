using MediatR;

namespace Conreign.Framework.Contracts.Communication
{
    public interface IMessage<out TPayload, out TMeta, out TOut> : IAsyncRequest<TOut>
    {
        TPayload Payload { get; }

        TMeta Meta { get; }
    }
}