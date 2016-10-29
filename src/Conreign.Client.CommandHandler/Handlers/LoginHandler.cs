using System.Threading.Tasks;
using Conreign.Client.CommandHandler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;

namespace Conreign.Client.CommandHandler.Handlers
{
    internal class LoginHandler : ICommandHandler<LoginCommand, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(CommandEnvelope<LoginCommand, LoginCommandResponse> message)
        {
            var context = message.Context;
            var result = await context.Connection.Login();
            var response = new LoginCommandResponse { AccessToken = result.AccessToken };
            return response;
        }
    }
}
