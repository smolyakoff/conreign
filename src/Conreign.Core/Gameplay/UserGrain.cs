using System;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace Conreign.Core.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        private static bool _universeActivated;
        private OrleansUserContext _context;
        private Topic _globalTopic;

        public override async Task OnActivateAsync()
        {
            _context = new OrleansUserContext();
            await EnsureUniverseActivated();
            _globalTopic = Topic.Global(GetStreamProvider(StreamConstants.ProviderName));
            await base.OnActivateAsync();
        }

        public async Task<IPlayer> JoinRoom(string roomId)
        {
            var player = GrainFactory.GetGrain<IPlayerGrain>(this.GetPrimaryKey(), roomId, null);
            var @event = new Connected(_context.ConnectionId, TopicIds.Player(_context.UserId, roomId));
            await player.Handle(@event);
            await _globalTopic.Send(@event);
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
