using MediatR;

namespace Conreign.Client
{
    internal interface ICommandHandler<TRequest, TResponse> :
        IAsyncRequestHandler<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        
    }
}