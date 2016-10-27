using MediatR;

namespace Conreign.Core.Client
{
    internal interface ICommandHandler<TRequest, TResponse> :
        IAsyncRequestHandler<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        
    }
}