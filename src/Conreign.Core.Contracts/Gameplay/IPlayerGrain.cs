using Conreign.Core.Contracts.Communication;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerGrain : IGrainWithStringKey, IPlayer, IChannel
    {
    }
}