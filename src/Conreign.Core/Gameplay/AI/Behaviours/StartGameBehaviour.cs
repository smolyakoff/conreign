using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
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

        public async Task Handle(IBotNotification<PlayerJoined> notification)
        {
            var context = notification.Context;
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
            _started = true;
            //await Task.Delay(1000);
            await context.Player.StartGame();
        }
    }
}
