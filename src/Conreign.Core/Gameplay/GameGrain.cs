using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay.Battle;
using Conreign.Core.Presence;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class GameGrain : Grain<GameState>, IGameGrain
    {
        private const int TurnLengthInTicks = 12;
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(5);

        private Game _game;
        private int _tick;
        private IDisposable _timer;

        public async Task Initialize(InitialGameData data)
        {
            await _game.Initialize(data);
            var gameStarted = new GameStarted.Server(this.AsReference<IGameGrain>());
            await _game.NotifyEverybodyExcept(data.InitiatorId, gameStarted);
            await WriteStateAsync();
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
            if (!_game.IsOnlinePlayersThinking)
            {
                await CalculateTurnInternal();
            }
            if (_game.IsEnded)
            {
                DeactivateOnIdle();
                await ClearStateAsync();
            }
        }

        public Task Notify(ISet<Guid> userIds, params IEvent[] events)
        {
            return _game.Notify(userIds, events);
        }

        public Task NotifyEverybody(params IEvent[] events)
        {
            return _game.NotifyEverybody(events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
        {
            return _game.NotifyEverybodyExcept(userIds, events);
        }

        public Task Connect(Guid userId, Guid connectionId)
        {
            return _game.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _game.Disconnect(userId, connectionId);
        }

        public override Task OnActivateAsync()
        {
            State.RoomId = this.GetPrimaryKeyString();
            var topic = Topic.Room(GetStreamProvider(StreamConstants.ProviderName), this.GetPrimaryKeyString());
            _game = new Game(State, topic, new CoinBattleStrategy());
            return base.OnActivateAsync();
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
                await _game.NotifyEverybody(new GameTicked(State.RoomId, _tick));
                _tick++;
            }
        }

        private async Task CalculateTurnInternal()
        {
            StopTimer();
            await _game.CalculateTurn();
            await WriteStateAsync();
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