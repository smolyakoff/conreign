using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay.Battle;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class GameGrain : Grain<GameState>, IGameGrain
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(5);
        private const int TurnLengthInTicks = 12;

        private Game _game;
        private IDisposable _timer;
        private int _tick;

        public override Task OnActivateAsync()
        {
            _game = new Game(State, new CoinBattleStrategy());
            return base.OnActivateAsync();
        }

        public async Task Initialize(InitialGameData data)
        {
            await _game.Initialize(data);
            ScheduleTimer();
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            return _game.GetState(userId);
        }

        public Task LaunchFleet(Guid userId, FleetData fleet)
        {
            return _game.LaunchFleet(userId, fleet);
        }

        public Task CancelFleet(Guid userId, FleetCancelationData fleet)
        {
            return _game.CancelFleet(userId, fleet);
        }

        public async Task EndTurn(Guid userId)
        {
            await _game.EndTurn(userId);
            if (!_game.IsAnybodyThinking)
            {
                await CalculateTurnInternal();
            }
        }

        public Task Notify(ISet<Guid> users, params IEvent[] events)
        {
            return _game.Notify(users, events);
        }

        public Task NotifyEverybody(params IEvent[] events)
        {
            return _game.NotifyEverybody(events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            return _game.NotifyEverybodyExcept(users, events);
        }

        public Task Join(Guid userId, IPublisher<IEvent> publisher)
        {
            return _game.Join(userId, publisher);
        }

        public Task Leave(Guid userId)
        {
            return _game.Leave(userId);
        }

        private async Task Tick(object arg)
        {
            if (_timer == null)
            {
                return;
            }
            if (_game.IsEnded)
            {
                StopTimer();
                return;
            }
            if (_tick == TurnLengthInTicks)
            {
                await CalculateTurnInternal();
            }
            else
            {
                await _game.NotifyEverybody(new GameTicked(_tick));
                _tick++;
            }
        }

        private async Task CalculateTurnInternal()
        {
            StopTimer();
            await _game.CalculateTurn();
            ScheduleTimer();
        }

        private void ScheduleTimer()
        {
            _timer = RegisterTimer(Tick, null, TimeSpan.Zero, TickInterval);
        }

        private void StopTimer()
        {
            _tick = 0;
            _timer?.Dispose();
            _timer = null;
        }
    }
}
