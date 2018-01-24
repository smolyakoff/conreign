using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IBotGrain : 
        IGrainWithGuidCompoundKey,
        IEventHandler<GameStarted>, 
        IEventHandler<TurnCalculationEnded>, 
        IEventHandler<PlayerDead>
    {
        Task EnsureIsListening();
    }
}
