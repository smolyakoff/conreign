using System;
using MediatR;

namespace Conreign.Client.Handler
{
    public class CommandEnvelope<TRequest, TResponse> : IRequest<TResponse> where TRequest : IRequest<TResponse>
    {
        public CommandEnvelope(TRequest command, HandlerContext context)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Command = command;
        }

        public TRequest Command { get; }
        public HandlerContext Context { get; }
    }
}