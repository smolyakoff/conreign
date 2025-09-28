using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class LoginHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    private readonly HandlerContext _context;

    public LoginHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _context.Connection.Login(command.AccessToken);
        var response = new LoginCommandResponse { AccessToken = result.AccessToken };
        return response;
    }
}