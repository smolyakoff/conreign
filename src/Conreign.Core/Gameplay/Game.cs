using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;

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

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new NotImplementedException();
        }


        public Task LaunchFleet(Guid userId, FleetData fleet)
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
    }
}
