using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IWorldGrain : IGrainWithIntegerKey, IWorld
    {
    }
}