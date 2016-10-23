using System;
using System.Threading.Tasks;
using Conreign.Core.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class LoginHandler : IAsyncRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly IHandlerContext _context;

        public LoginHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand message)
        {
            var result =  await _context.Connection.Login();
            var response = new LoginCommandResponse {AccessToken = result.AccessToken};
            return response;
        }
    }
}
