using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class BattleBehaviour :
        IBotBehaviour<TurnCalculationEnded>, 
        IBotBehaviour<GameStarted>, 
        IBotBehaviour<PlayerDead>,
        IBotBehaviour<GameEnded>
    {
        private MapData _map;
        private bool _ended;
        private readonly IBotBattleStrategy _strategy;

        public BattleBehaviour(IBotBattleStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }
            _strategy = strategy;
        }

        public async Task Handle(TurnCalculationEnded @event, BotContext context)
        {
            if ( _ended || context.Player == null)
            {
                return;
            }
            _map = @event.Map;
            await Think(context);
        }

        public async Task Handle(GameStarted @event, BotContext context)
        {
            if (context.Player == null)
            {
                return;
            }
            var game = (GameData) await context.Player.GetState();
            _map = game.Map;
            await Think(context);
        }

        public Task Handle(PlayerDead @event, BotContext context)
        {
            if (@event.UserId != context.UserId)
            {
                return Task.CompletedTask;
            }
            _ended = true;
            return Task.CompletedTask;
        }

        public Task Handle(GameEnded @event, BotContext context)
        {
            _ended = true;
            return Task.CompletedTask;
        }

        private async Task Think(BotContext context)
        {
            var map = new ReadOnlyMap(new Map(_map));
            var fleets = _strategy.ChooseFleetsToLaunch(context.UserId, map);
            var tasks = fleets.Select(x => context.Player.LaunchFleet(x));
            await Task.WhenAll(tasks);
            await context.Player.EndTurn();
        }
    }
}