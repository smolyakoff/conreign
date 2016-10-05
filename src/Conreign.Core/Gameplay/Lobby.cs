using System;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Commands;

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

        public Task Join(JoinCommand command)
        {
            return _hub.Join(command);
        }

        public Task Leave(LeaveCommand command)
        {
            return _hub.Leave(command);
        }

        public Task Notify(NotifyCommand command)
        {
            return _hub.Notify(command);
        }

        public Task NotifyEverybody(NotifyEverybodyCommand command)
        {
            return _hub.NotifyEverybody(command);
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGameSettings()
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayerOptions(UpdatePlayerOptionsCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<IGame> StartGame(StartGameCommand command)
        {
            return _gameFactory.CreateGame();
        }
    }
}