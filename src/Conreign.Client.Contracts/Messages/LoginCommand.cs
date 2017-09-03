using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    [SkipAuthentication]
    public class LoginCommand : IRequest<LoginCommandResponse>
    {
        public string AccessToken { get; set; }
    }
}