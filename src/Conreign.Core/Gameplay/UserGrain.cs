using System;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Gameplay
{
    [StatelessWorker]
    public class UserGrain : Grain<Guid>, IUserGrain
    {
        private User _user;
        private static bool _universeActivated;

        public override async Task OnActivateAsync()
        {
            var context = new OrleansUserContext();
            _user = new User(
                context, 
                this.AsReference<IUserGrain>(), 
                this.AsReference<IUserGrain>());
            await EnsureUniverseActivated();
            await base.OnActivateAsync();
        }

        public Task<IPlayer> JoinRoom(string roomId)
        {
            return _user.JoinRoom(roomId);
        }

        public Task<IPlayer> CreatePlayer(string roomId)
        {
            var player = GrainFactory.GetGrain<IPlayerGrain>(this.GetPrimaryKey(), roomId, null);
            return Task.FromResult<IPlayer>(player);
        }

        public Task<IPublisher<IServerEvent>> CreateSystemPublisher(string topic)
        {
            var publisher = GrainFactory.GetGrain<IBusGrain>(topic);
            return Task.FromResult((IPublisher<IServerEvent>)publisher);
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
