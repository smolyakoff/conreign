using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;


namespace Conreign.Core.Gameplay
{
    public class Lobby : ILobby
    {
        private readonly Hub _hub;
        private LobbyState _state;
        private readonly IGameFactory _gameFactory;

        public Lobby(LobbyState state, IGameFactory gameFactory)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (gameFactory == null)
            {
                throw new ArgumentNullException(nameof(gameFactory));
            }
            _hub = new Hub(state.Hub);
            _state = state;
            _gameFactory = gameFactory;
        }

        public Task Notify(object @event, ISet<Guid> users)
        {
            return _hub.Notify(@event, users);
        }

        public Task NotifyEverybody(object @event)
        {
            return _hub.NotifyEverybody(@event);
        }

        public Task Join(Guid userId, IObserver observer)
        {
            return _hub.Join(userId, observer);
        }

        public Task Leave(Guid userId)
        {
            return _hub.Leave(userId);
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task<IGame> StartGame(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}