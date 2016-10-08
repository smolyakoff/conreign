using System;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Contracts;
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

        public override Task OnActivateAsync()
        {
            var universe = GrainFactory.GetGrain<IUniverseGrain>(default(long));
            var context = new OrleansUserContext();
            _user = new User(context, universe, this);
            return base.OnActivateAsync();
        }

        public Task<IPlayer> JoinRoom(string roomId)
        {
            return _user.JoinRoom(roomId);
        }

        public Task<IConnectablePlayer> Create(string roomId)
        {
            var player = GrainFactory
                .GetGrain<IPlayerGrain>(this.GetPrimaryKey(), roomId, null);
            return Task.FromResult<IConnectablePlayer>(player);
        }
    }
}
