using System;
using System.Reflection;
using System.Threading.Tasks;
using Conreign.Client.CommandHandler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Client.CommandHandler.Handlers.Decorators
{
    internal class AuthenticationDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> _next;
        private readonly bool _skipAuthentication;

        public AuthenticationDecorator(IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _skipAuthentication = typeof(TCommand).GetCustomAttribute<SkipAuthenticationAttribute>() != null;
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message)
        {
            var context = message.Context;
            if (_skipAuthentication)
            {
                return await _next.Handle(message);
            }
            var loginResult = await context.Connection.Authenticate(context.AccessToken);
            context.UserId = loginResult.UserId;
            context.User = loginResult.User;
            return await _next.Handle(message);
        }
    }
}