using MediatR;

namespace Conreign.Client.Handler
{
    internal interface ICommandHandler<TRequest, TResponse> :
        IAsyncRequestHandler<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IRequest<TResponse>
    {
    }
}