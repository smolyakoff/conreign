using System.Reflection;
using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;
using MediatR;

namespace Conreign.Client.Handler.Behaviours
{
    public class AuthenticationBehaviour<TCommand, TResponse> :
        ICommandPipelineBehaviour<TCommand, TResponse> where TCommand : IRequest<TResponse>
    {
        private readonly bool _skipAuthentication;

        public AuthenticationBehaviour()
        {
            _skipAuthentication = typeof(TCommand).GetCustomAttribute<SkipAuthenticationAttribute>() != null;
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = message.Context;
            if (_skipAuthentication)
            {
                return await next.Invoke();
            }
            var loginResult = await context.Connection.Authenticate(context.Metadata.AccessToken);
            context.UserId = loginResult.UserId;
            context.User = loginResult.User;
            return await next.Invoke();
        }
    }
}