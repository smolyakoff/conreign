using System;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Auth.Data;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Utility;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Game
{
    [StatelessWorker]
    public class WorldGrain : Grain, IWorldGrain
    {
        private IPlayerMembershipGrain _membership;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            _membership = GrainFactory.GetGrain<IPlayerMembershipGrain>(default(long));
        }

        public async Task<WelcomeMessagePayload> Arrive(ArriveAction action)
        {
            var playerKey = action.Meta?.User?.UserKey ?? Guid.NewGuid();
            var accessToken = action.Meta?.Auth?.IsAuthenticated == true
                ? action.Meta?.Auth?.AccessToken
                : (await LoginAnonymous(playerKey))?.AccessToken;
            var getOrCreatePlayer = new GetPlayerSettingsAction(playerKey);
            var settings = await _membership.GetPlayerSettings(getOrCreatePlayer);
            var galaxyNameGenerator = new GalaxyNameGenerator();
            var galaxyName = await galaxyNameGenerator.Generate();
            var player = new WelcomeMessagePayload
            {
                AccessToken = accessToken,
                PlayerName = settings.Name,
                GalaxyName = galaxyName
            };
            return player;
        }

        public async Task Connect(ConnectAction action)
        {
            action.EnsureNotNull().EnsureAuthenticated();
            await _membership.Connect(action);
        }

        public async Task Disconnect(DisconnectAction action)
        {
            action.EnsureNotNull().EnsureAuthenticated();
            await _membership.Disconnect(action);
        }

        public Task<GameStatusPayload> CheckGameStatus(CheckGameStatusAction action)
        {
            throw new System.NotImplementedException();
        }

        public Task<GameRoomPayload> ReserveGameRoom(ReserveGameRoomAction reservation)
        {
            throw new System.NotImplementedException();
        }

        private async Task<AccessTokenPayload> LoginAnonymous(Guid playerKey)
        {
            var auth = GrainFactory.GetGrain<IAuthGrain>(default(long));
            var token = await auth.LoginAnonymous(new LoginAnonymousAction(playerKey));
            return token;
        }
    }
}
