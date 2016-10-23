using System.Reflection;
using System.Threading.Tasks;
using Conreign.Core.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers.Decorators
{
    public class AuthenticationDecorator<TCommand, TResponse> : IAsyncRequestHandler<TCommand, TResponse> where TCommand : IAsyncRequest<TResponse>
    {
        private readonly IHandlerContext _context;
        private readonly IAsyncRequestHandler<TCommand, TResponse> _next;
        private readonly bool _skipAuthentication;

        public AuthenticationDecorator(IHandlerContext context, IAsyncRequestHandler<TCommand, TResponse> next)
        {
            _context = context;
            _next = next;
            _skipAuthentication = typeof(TCommand).GetCustomAttribute<SkipAuthenticationAttribute>() != null;
        }

        public async Task<TResponse> Handle(TCommand command)
        {
            if (_skipAuthentication)
            {
                return await _next.Handle(command);
            }
            var loginResult = await _context.Connection.Authenticate(_context.AccessToken);
            _context.UserId = loginResult.UserId;
            _context.User = loginResult.User;
            return await _next.Handle(command);
        }
    }
}