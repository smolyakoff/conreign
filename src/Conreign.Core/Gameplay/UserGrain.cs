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
        private IAsyncStream<IServerEvent> _globalStream;

        public override async Task OnActivateAsync()
        {
            _context = new OrleansUserContext();
            await EnsureUniverseActivated();
            _globalStream = GetStreamProvider(StreamConstants.ClientStreamProviderName)
                    .GetStream<IServerEvent>(default(Guid), ServerTopics.Global);
            await base.OnActivateAsync();
        }

        public async Task<IPlayer> JoinRoom(string roomId)
        {
            var player = GrainFactory.GetGrain<IPlayerGrain>(this.GetPrimaryKey(), roomId, null);
            var stream = GetStreamProvider(StreamConstants.ClientStreamProviderName)
                .GetStream<Disconnected>(Guid.Empty, ServerTopics.Player(this.GetPrimaryKey(), roomId));
            var @event = new Connected(_context.ConnectionId, new Publisher<Disconnected>(stream));
            await player.Handle(@event);
            await _globalStream.OnNextAsync(@event);
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
