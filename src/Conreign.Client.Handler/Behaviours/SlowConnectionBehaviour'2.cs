using System;
using System.Threading.Tasks;
using MediatR;

namespace Conreign.Client.Handler.Behaviours
{
    public class SlowConnectionBehaviour<TCommand, TResponse> :
        ICommandPipelineBehaviour<TCommand, TResponse> where TCommand : IRequest<TResponse>
    {
        private readonly SlowConnectionBehaviourOptions _options;

        public SlowConnectionBehaviour(SlowConnectionBehaviourOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> request,
            RequestHandlerDelegate<TResponse> next)
        {
            await Task.Delay(_options.Delay);
            return await next();
        }
    }
}