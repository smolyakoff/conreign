using MediatR;

namespace Conreign.Client.Handler.Handlers.Common
{
    internal interface ICommandPipelineBehaviour<TRequest, TResponse> 
        : IPipelineBehavior<CommandEnvelope<TRequest, TResponse>, TResponse> where TRequest : IRequest<TResponse>
    {
    }
}