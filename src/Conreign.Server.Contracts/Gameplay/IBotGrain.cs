using System.Threading.Tasks;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IBotGrain : 
        IGrainWithGuidCompoundKey,
        IBot
    {
        Task EnsureIsListening();
    }
}
