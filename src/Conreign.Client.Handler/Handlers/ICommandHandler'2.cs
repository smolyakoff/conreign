using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal interface ICommandHandler<TRequest, TResponse> :
        IAsyncRequestHandler<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IRequest<TResponse>
    {
    }
}