using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IGameGrain : IGrainWithStringKey
    {
        Task MakeTurn(MakeTurnAction action);
    }
}