using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.AI.Battle;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay;
using Serilog;

namespace Conreign.Core.AI.Behaviours
{
    public class BattleBehaviour :
        IBotBehaviour<TurnCalculationEnded>,
        IBotBehaviour<GameStarted>,
        IBotBehaviour<PlayerDead>,
        IBotBehaviour<GameEnded>
    {
        private readonly IBotBattleStrategy _strategy;
        private bool _ended;
        private MapData _map;

        public BattleBehaviour(IBotBattleStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }
            _strategy = strategy;
        }

        public Task Handle(IBotNotification<GameEnded> notification)
        {
            _ended = true;
            return TaskCompleted.Completed;
        }

        public async Task Handle(IBotNotification<GameStarted> notification)
        {
            var context = notification.Context;
            if (context.Player == null)
            {
                return;
            }
            var game = (GameData) await context.Player.GetState();
            _map = game.Map;
            await Think(context);
        }

        public Task Handle(IBotNotification<PlayerDead> notification)
        {
            var context = notification.Context;
            var @event = notification.Event;
            if (@event.UserId != context.UserId)
            {
                return TaskCompleted.Completed;
            }
            _ended = true;
            return TaskCompleted.Completed;
        }

        public async Task Handle(IBotNotification<TurnCalculationEnded> notification)
        {
            var context = notification.Context;
            var @event = notification.Event;
            if (_ended || context.Player == null)
            {
                return;
            }
            _map = @event.Map;
            using (context.Logger.BeginTimedOperation("Bot.Think"))
            {
                await Think(context);
            };
        }

        private async Task Think(BotContext context)
        {
            if (context.UserId == null)
            {
                throw new InvalidOperationException("Expected to be authenticated already.");
            }
            var map = new ReadOnlyMap(new Map(_map));
            var fleets = _strategy.ChooseFleetsToLaunch(context.UserId.Value, map);
            var tasks = fleets.Select(x => context.Player.LaunchFleet(x));
            await Task.WhenAll(tasks);
            await context.Player.EndTurn();
        }
    }
}