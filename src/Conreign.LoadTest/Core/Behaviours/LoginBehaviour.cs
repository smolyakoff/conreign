using System.Threading.Tasks;
using Conreign.LoadTest.Core.Behaviours.Events;
using Conreign.LoadTest.Core.Events;

namespace Conreign.LoadTest.Core.Behaviours
{
    public class LoginBehaviour : IBotBehaviour<BotStarted>
    {
        public async Task Handle(IBotNotification<BotStarted> notification)
        {
            var context = notification.Context;
            var loginResult = await context.Connection.Login();
            context.UserId = loginResult.UserId;
            context.User = loginResult.User;
            context.Logger = context.Logger.ForContext("UserId", context.UserId.Value);
            context.Notify(new BotAuthenticated());
        }
    }
}