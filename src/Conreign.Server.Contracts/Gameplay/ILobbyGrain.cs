using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface ILobbyGrain : 
        IGrainWithStringKey,
        ILobby
    {
    }
}