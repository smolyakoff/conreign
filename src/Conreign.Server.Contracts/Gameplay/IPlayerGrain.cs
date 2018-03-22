using System.Threading.Tasks;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayerGrain :
        IGrainWithGuidCompoundKey,
        IPlayer
    {
        Task EnsureIsListening();
    }
}