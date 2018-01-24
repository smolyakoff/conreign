using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Server.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        public async Task<IPlayer> JoinRoom(string roomId, Guid connectionId)
        {
            var userId = this.GetPrimaryKey();
            var connection = GrainFactory.GetGrain<IConnectionGrain>(connectionId);
            var topicId = TopicIds.Player(userId, roomId);
            var player = GrainFactory.GetGrain<IPlayerGrain>(userId, roomId, null);
            await player.EnsureIsListening();
            await connection.Connect(topicId);
            return player;
        }
    }
}