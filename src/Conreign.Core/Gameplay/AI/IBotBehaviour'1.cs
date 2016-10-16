using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Gameplay.AI
{
    public interface IBotBehaviour<in T> : IBotBehaviour where T : IClientEvent
    {
        Task Handle(T @event, BotContext context);
    }
}