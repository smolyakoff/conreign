using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
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
        public Task<PlayerPayload> Arrive(ArriveAction action)
        {
            var auth = GrainFactory.GetGrain<IAuthGrain>(0);
            //var abc = auth.AuthenticateAnonymousUser();
            return Task.FromResult(new PlayerPayload {Token = "abc"});
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
