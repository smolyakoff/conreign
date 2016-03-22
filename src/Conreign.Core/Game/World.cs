using System;
using System.Threading.Tasks;
using Conreign.Core.Auth.Actions;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using MediatR;

namespace Conreign.Core.Game
{
    public class World : IAsyncRequestHandler<ArriveAction, WelcomeMessagePayload>
    {
        private readonly IMediator _mediator;

        private readonly GameRoomKeyGenerator _gameRoomKeyGenerator;

        public World(IMediator mediator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            _mediator = mediator;
            _gameRoomKeyGenerator = new GameRoomKeyGenerator();
        }

        public async Task<WelcomeMessagePayload> Handle(ArriveAction message)
        {
            var playerKey = message.Meta?.User?.UserKey ?? Guid.NewGuid();
            string accessToken;
            if (message.Meta?.Auth?.IsAuthenticated == true)
            {
                accessToken = message.Meta?.Auth?.AccessToken;
            }
            else
            {
                var loginMessage = new LoginAnonymousAction(playerKey);
                var loginResult = await _mediator.SendAsync(loginMessage);
                accessToken = loginResult.AccessToken;
            }
            var settingsMessage = new GetPlayerSettingsAction(playerKey);
            var settings = await _mediator.SendAsync(settingsMessage);
            var player = new WelcomeMessagePayload
            {
                AccessToken = accessToken,
                PlayerSettings = settings,
                SuggestedGameRoom = null
            };
            return player;
        }
    }
}