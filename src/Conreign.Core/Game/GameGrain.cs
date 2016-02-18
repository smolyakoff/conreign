using System.Threading.Tasks;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;

namespace Conreign.Core.Game
{
    public class GameGrain : Grain, IGameGrain
    {
        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public Task SendChatMessage(SendChatMessageAction action)
        {
            throw new System.NotImplementedException();
        }

        public Task MakeTurn(MakeTurnAction action)
        {
            throw new System.NotImplementedException();
        }
    }
}
