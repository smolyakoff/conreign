using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Battle;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Presence;
using Orleans;
using Serilog;

namespace Conreign.Server.Gameplay
{
    public class GameGrain : Grain<GameState>, IGameGrain
    { 
        private readonly GameOptions _options;
        private Game _game;
        private int _tick;
        private IDisposable _timer;
        private ILogger _logger;

        public GameGrain(ILogger logger, GameOptions options)
        {
            _logger = logger.ForContext(GetType());
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Start(Guid userId, InitialGameData data)
        {
            var botGrains = data.Players
                .Where(x => x.Type == PlayerType.Bot)
                .Select(x => GrainFactory.GetGrain<IBotGrain>(x.UserId, State.RoomId, null));
            await Task.WhenAll(botGrains.Select(x => x.EnsureIsListening()));
            await _game.Start(userId, data);
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

        public Task SendMessage(Guid userId, TextMessageData textMessage)
        {
            return _game.SendMessage(userId, textMessage);
        }

        public Task LaunchFleet(Guid userId, FleetData fleet)
        {
            return _game.LaunchFleet(userId, fleet);
        }

        public Task EndTurn(Guid userId, List<FleetData> fleets)
        {
            return _game.EndTurn(userId, fleets);
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
                await CalculateTurn();
            }
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
            var battleStrategy = new CoinBattleStrategy();
            var hub = new Hub(State.Hub, topic, new GameBotUserIdSet(State));
            _game = new Game(State, _options, hub, battleStrategy);
            return base.OnActivateAsync();
        }

        private async Task Tick(object arg)
        {
            if (_timer == null)
            {
                _logger.Warning($"{nameof(_timer)} is null in {nameof(Tick)} method.");
                return;
            }
            _tick = await _game.ProcessTick(_tick);
            if (_tick == 0)
            {
                await CalculateTurn();
            }
        }

        private async Task CalculateTurn()
        {
            StopTimer();
            var outcome = await _game.CalculateTurn();
            switch (outcome)
            {
                case GameStalledTurnOutcome stalled:
                    _logger.Information(
                        "Going to deactivate game due to inactivity. Inactivity period was {InactivityPeriod}.",
                        stalled.InactivityPeriod);
                    break;
                case GameEndedTurnOutcome ended:
                    _logger.Information("Game ended in {TurnCount} turns.", ended.TurnsCount);
                    break;
            }
            if (outcome is TurnCompletedTurnOutcome)
            {
                await WriteStateAsync();
                ScheduleTimer();
                return;
            }
            await ClearStateAsync();
            DeactivateOnIdle();
        }

        private void ScheduleTimer()
        {
            _timer = RegisterTimer(
                Tick,
                state: null,
                dueTime: _options.TickInterval,
                period: _options.TickInterval);
        }

        private void StopTimer()
        {
            _tick = 0;
            _timer?.Dispose();
            _timer = null;
        }
    }
}