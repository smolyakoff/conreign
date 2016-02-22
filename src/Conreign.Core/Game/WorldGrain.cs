using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Game
{
    [StatelessWorker]
    public class WorldGrain : Grain, IWorldGrain
    {
        public async Task<PlayerPayload> Arrive(ArriveAction action)
        {
            var auth = GrainFactory.GetGrain<IAuthGrain>(default(long));
            var token = await auth.LoginAnonymous(new LoginAnonymousAction());
            var player = new PlayerPayload
            {
                AccessToken = token.AccessToken,
                Settings = new PlayerSettingsPayload()
            };
            return player;
        }

        public Task<GameStatusPayload> CheckGameStatus(CheckGameStatusAction action)
        {
            throw new System.NotImplementedException();
        }

        public Task<GameRoomPayload> ReserveGameRoom(ReserveGameRoomAction reservation)
        {
            throw new System.NotImplementedException();
        }
    }
}
