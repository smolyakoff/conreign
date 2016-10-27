using System.Threading.Tasks;
using Conreign.Core.Gameplay.AI.Behaviours.Events;
using Conreign.Core.Gameplay.AI.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
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
