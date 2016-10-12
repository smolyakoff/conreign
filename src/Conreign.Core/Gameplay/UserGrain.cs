using System;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Communication;
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

        public override Task OnActivateAsync()
        {
            var context = new OrleansUserContext();
            _user = new User(
                context, 
                this.AsReference<IUserGrain>(), 
                this.AsReference<IUserGrain>());
            return base.OnActivateAsync();
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

        public async Task<ISystemPublisher> CreateSystemPublisher(string topic)
        {
            if (topic == SystemTopics.Global)
            {
                await EnsureUniverseActivated();
            }
            var publisher = GrainFactory.GetGrain<IBusGrain>(topic);
            return publisher;
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
