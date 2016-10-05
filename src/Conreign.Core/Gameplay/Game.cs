using System;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay
{
    public class Game : IGame
    {
        private readonly GameState _state;
        private readonly Hub _hub;

        public Game(GameState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _hub = new Hub(state.Hub);
            _state = state;
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

        public Task LaunchFleet()
        {
            throw new NotImplementedException();
        }

        public Task EndTurn()
        {
            throw new NotImplementedException();
        }

        internal Task CalculateTurn()
        {
            return Task.CompletedTask;
        }
    }
}
