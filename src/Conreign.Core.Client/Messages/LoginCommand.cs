using MediatR;

namespace Conreign.Core.Client.Messages
{
    [SkipAuthentication]
    public class LoginCommand : IAsyncRequest<LoginCommandResponse>
    {
    }
}
