using System.Reflection;
using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Behaviours;

public class AuthenticationBehaviour<TCommand, TResponse> :
    IPipelineBehavior<TCommand, TResponse> where TCommand : IRequest<TResponse>
{
    private readonly HandlerContext _context;
    private readonly bool _skipAuthentication;

    public AuthenticationBehaviour(HandlerContext context)
    {
        _context = context;
        _skipAuthentication = typeof(TCommand).GetCustomAttribute<SkipAuthenticationAttribute>() != null;
    }

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_skipAuthentication)
        {
            return await next.Invoke();
        }

        var loginResult = await _context.Connection.Authenticate(_context.Metadata.AccessToken);
        _context.UserId = loginResult.UserId;
        _context.User = loginResult.User;
        return await next.Invoke();
    }
}