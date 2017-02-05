using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    [SkipAuthentication]
    public class LoginCommand : IRequest<LoginCommandResponse>
    {
        public string AccessToken { get; set; }
    }
}