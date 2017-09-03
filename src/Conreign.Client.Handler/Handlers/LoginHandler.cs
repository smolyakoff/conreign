using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;

namespace Conreign.Client.Handler.Handlers
{
    internal class LoginHandler : ICommandHandler<LoginCommand, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(CommandEnvelope<LoginCommand, LoginCommandResponse> message)
        {
            var context = message.Context;
            var command = message.Command;
            var result = await context.Connection.Login(command.AccessToken);
            var response = new LoginCommandResponse {AccessToken = result.AccessToken};
            return response;
        }
    }
}