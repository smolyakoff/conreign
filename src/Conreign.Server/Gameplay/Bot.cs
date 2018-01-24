using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Core.Battle.AI;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;

namespace Conreign.Server.Gameplay
{
    public class Bot : IEventHandler<GameStarted>, IEventHandler<TurnCalculationEnded>, IEventHandler<PlayerDead>
    {
        private readonly IGame _game;
        private readonly IBotBattleStrategy _battleStrategy;
        private readonly BotState _state;
        private BotMap _map;

        public Bot(BotState state, IGame game, IBotBattleStrategy battleStrategy)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _battleStrategy = battleStrategy ?? throw new ArgumentNullException(nameof(battleStrategy));
            _map = new BotMap(_state.Map);
        }

        public async Task Handle(GameStarted @event)
        {
            _state.Map = await _game.GetMap();
            _map = new BotMap(_state.Map);
            await Think();
        }

        public Task Handle(TurnCalculationEnded @event)
        {
            var map = EnsureMapIsInitialized();
            map.UpdatePlanets(@event.Map.Planets.Values);
            return Think();
        }

        public Task Handle(PlayerDead @event)
        {
            if (@event.UserId == _state.UserId)
            {
                _state.IsDead = true;
            }
            return Task.CompletedTask;
        }

        private Task Think()
        {
            if (_state.IsDead)
            {
                return Task.CompletedTask;
            }
            var map = EnsureMapIsInitialized();
            var fleets = _battleStrategy.ChooseFleetsToLaunch(_state.UserId, map);
            return _game.EndTurn(_state.UserId, fleets);
        }

        private BotMap EnsureMapIsInitialized()
        {
            if (_map == null)
            {
                throw new InvalidOperationException("Expected map to be initialized.");
            }
            return _map;
        }
    }
}
