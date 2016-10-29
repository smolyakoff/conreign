using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    [SkipAuthentication]
    public class LoginCommand : IAsyncRequest<LoginCommandResponse>
    {
    }
}