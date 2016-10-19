using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI
{
    public class StartGameBehaviour : IBotBehaviour<PlayerJoined>
    {
        private readonly int _targetPlayersCount;
        private int? _currentCount;
        private bool _started;

        public StartGameBehaviour(int targetPlayersCount)
        {
            _targetPlayersCount = targetPlayersCount;
        }

        public async Task Handle(PlayerJoined @event, BotContext context)
        {
            if (_started || context.Player == null)
            {
                return;
            }

            if (_currentCount == null)
            {
                var game =  await context.Player.GetState();
                _currentCount = game.Players.Count;
            }
            else
            {
                _currentCount++;
            }
            if (_currentCount < _targetPlayersCount)
            {
                return;
            }
            if (_started)
            {
                return;
            }
            _started = true;
            await context.Player.StartGame();
            var st = await context.Player.GetState();
        }
    }
}
