using System.Threading.Tasks;

namespace Conreign.Core.Gameplay.AI
{
    public interface ILifetimeBotBehaviour : IBotBehaviour
    {
        Task Start(BotContext context);
    }
}