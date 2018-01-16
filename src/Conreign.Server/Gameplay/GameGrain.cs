using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Core.Battle;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Orleans;
using Serilog;

namespace Conreign.Server.Gameplay
{
    public class GameGrain : Grain<GameState>, IGameGrain
    {
        private readonly GameGrainOptions _options;
        private Game _game;
        private int _tick;
        private IDisposable _timer;
        private ILogger _logger;

        public GameGrain(ILogger logger, GameGrainOptions options)
        {
            _logger = logger.ForContext(GetType());
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Initialize(InitialGameData data)
        {
            await _game.Initialize(data);
            await WriteStateAsync();
            _logger.Information(
                "Game initialized. Map size is {MapWidth}x{MapHeight}. There are {PlayerCount} players.",
                data.Map.Width,
                data.Map.Height,
                data.Players.Count);
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
            _logger = _logger.ForContext(nameof(State.RoomId), State.RoomId);
            _game = new Game(State, topic, new CoinBattleStrategy());
            return base.OnActivateAsync();
        }

        private async Task Tick(object arg)
        {
            if (_timer == null)
            {
                return;
            }
            _tick++;
            if (_tick == _options.TurnLengthInTicks)
            {
                await CalculateTurnInternal();
            }
            else
            {
                await _game.NotifyEverybody(new GameTicked(State.RoomId, _tick));
            }
        }

        private async Task CalculateTurnInternal()
        {
            StopTimer();
            var isEnded = await _game.CalculateTurn();
            var isInactive = _game.EveryoneOfflinePeriod > _options.MaxInactivityPeriod;
            if (isEnded)
            {
                _logger.Information("Game ended in {TurnCount} turns.", _game.Turn + 1);
            }
            if (isInactive)
            {
                _logger.Information(
                    "Going to deactivate game due to inactivity. Inactivity period was {InactivityPeriod}.",
                    _game.EveryoneOfflinePeriod);
            }
            if (isEnded || isInactive)
            {
                await ClearStateAsync();
                DeactivateOnIdle();
                return;
            }
            await WriteStateAsync();
            ScheduleTimer();
        }

        private void ScheduleTimer()
        {
            _timer = RegisterTimer(
                Tick,
                null,
                _options.TickInterval,
                _options.TickInterval);
        }

        private void StopTimer()
        {
            _tick = 0;
            _timer?.Dispose();
            _timer = null;
        }
    }
}