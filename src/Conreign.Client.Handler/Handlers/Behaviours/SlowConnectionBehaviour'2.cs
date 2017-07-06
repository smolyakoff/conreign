using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using MediatR;

namespace Conreign.Client.Handler.Handlers.Behaviours
{
    internal class SlowConnectionBehaviour<TCommand, TResponse> : 
        ICommandPipelineBehaviour<TCommand, TResponse> where TCommand : IRequest<TResponse>
    {
        private readonly TimeSpan _delay;

        public SlowConnectionBehaviour(TimeSpan delay)
        {
            if (_delay.Ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay should be positive.");
            }
            _delay = delay;
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> request, RequestHandlerDelegate<TResponse> next)
        {
            await Task.Delay(_delay);
            return await next();
        }
    }
}
