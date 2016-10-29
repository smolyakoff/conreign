using MediatR;

namespace Conreign.Client.Handler.Handlers.Common
{
    internal interface ICommandHandler<TRequest, TResponse> :
        IAsyncRequestHandler<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
    }
}