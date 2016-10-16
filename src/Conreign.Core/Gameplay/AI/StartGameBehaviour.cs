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
            Console.WriteLine("Joining...");
            if (_started || context.Player == null)
            {
                Console.WriteLine("Exiting because of started or player null");
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
            Console.WriteLine($"Joined: {_currentCount}");
            if (_currentCount < _targetPlayersCount)
            {
                return;
            }
            if (_started)
            {
                return;
            }
            _started = true;
            Console.WriteLine("Starting game...");
            await context.Player.StartGame();
            Console.WriteLine("Started game");
        }
    }
}
