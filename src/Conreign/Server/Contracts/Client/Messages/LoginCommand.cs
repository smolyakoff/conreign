using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

[SkipAuthentication]
public class LoginCommand : IRequest<LoginCommandResponse>
{
    public string AccessToken { get; set; }
}