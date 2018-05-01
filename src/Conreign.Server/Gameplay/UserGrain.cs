using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Server.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        private Guid UserId => this.GetPrimaryKey();

        public async Task<IPlayerClient> JoinRoom(string roomId, Guid connectionId)
        {
            var connection = GrainFactory.GetGrain<IConnectionGrain>(connectionId);
            var player = await connection.Bind(UserId, roomId);
            return player;
        }
    }
}