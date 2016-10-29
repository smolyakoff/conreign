using System;
using MediatR;

namespace Conreign.Client.CommandHandler.Handlers.Common
{
    internal class CommandEnvelope<TRequest, TResponse> : IAsyncRequest<TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        public CommandEnvelope(TRequest command, IHandlerContext context)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
            Command = command;
        }

        public TRequest Command { get; }
        public IHandlerContext Context { get; }
    }
}
