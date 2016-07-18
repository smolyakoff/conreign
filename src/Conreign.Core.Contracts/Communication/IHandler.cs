using MediatR;

namespace Conreign.Core.Contracts.Communication
{
    public interface IHandler<TIn, TOut> : IAsyncRequestHandler<Message<TIn, TOut>, TOut>
    {
    }
}