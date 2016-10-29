using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        private readonly Dictionary<string, IPlayer> _players = new Dictionary<string, IPlayer>();

        public async Task<IPlayer> JoinRoom(string roomId, Guid connectionId)
        {
            if (_players.ContainsKey(roomId))
            {
                return _players[roomId];
            }
            var userId = this.GetPrimaryKey();
            var connection = GrainFactory.GetGrain<IConnectionGrain>(connectionId);
            var topicId = TopicIds.Player(userId, roomId);
            var player = GrainFactory.GetGrain<IPlayerGrain>(userId, roomId, null);
            await player.Listen();
            await connection.Connect(topicId);
            _players[roomId] = player;
            return player;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }
    }
}