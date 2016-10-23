using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        private static bool _universeActivated;
        private Topic _globalTopic;
        private readonly Dictionary<string, IPlayer> _players = new Dictionary<string, IPlayer>();

        public override async Task OnActivateAsync()
        {
            await EnsureUniverseActivated();
            _globalTopic = Topic.Global(GetStreamProvider(StreamConstants.ProviderName));
            await base.OnActivateAsync();
        }

        public async Task<IPlayer> JoinRoom(string roomId, Guid connectionId)
        {
            if (_players.ContainsKey(roomId))
            {
                return _players[roomId];
            }
            var userId = this.GetPrimaryKey();
            var player = GrainFactory.GetGrain<IPlayerGrain>(userId, roomId, null);
            var @event = new Connected(connectionId, TopicIds.Player(userId, roomId));
            await player.Handle(@event);
            await _globalTopic.Send(@event);
            _players[roomId] = player;
            return player;
        }

        private async Task EnsureUniverseActivated()
        {
            if (_universeActivated)
            {
                return;
            }
            var universe = GrainFactory.GetGrain<IUniverseGrain>(default(long));
            await universe.Ping();
            _universeActivated = true;
        }
    }
}
