using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Errors;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Contracts.Presence;
using Conreign.Core;
using Conreign.Core.Battle;
using Conreign.Core.Utility;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Contracts.Presence;
using Conreign.Server.Gameplay.Validators;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    public class Game : IGame
    {
        private readonly IBattleStrategy _battleStrategy;
        private readonly IGameState _state;
        private readonly GameOptions _options;
        private readonly IHub _hub;
        private Map _map;

        public Game(IGameState state, GameOptions options, IHub hub, IBattleStrategy battleStrategy)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _hub = hub;
            _map = new Map(_state.Map);
            _battleStrategy = battleStrategy ?? throw new ArgumentNullException(nameof(battleStrategy));
        }

        public bool IsOnlinePlayersThinking => _state.PlayerStates
            .Where(kv => _hub.IsOnline(kv.Key))
            .Select(kv => kv.Value)
            .Any(p => p.TurnStatus == TurnStatus.Thinking);

        public int Turn => _state.Turn;
        private bool IsInactive => EveryoneOfflinePeriod >= _options.MaxInactivityPeriod;
        private TimeSpan EveryoneOfflinePeriod => _hub.EveryoneOfflinePeriod;

        public Task Connect(Guid userId, Guid connectionId)
        {
            return _hub.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _hub.Disconnect(userId, connectionId);
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);
            var playerState = _state.PlayerStates.GetOrCreateDefault(userId);
            var data = new GameData
            {
                Players = _state.Players,
                Events = _hub.GetEvents(userId).Select(x => x.ToEnvelope()).ToList(),
                LeaderUserId = _hub.LeaderUserId,
                Map = _state.Map,
                MovingFleets = playerState.MovingFleets,
                WaitingFleets = playerState.WaitingFleets,
                PresenceStatuses = GetPresenceStatuses(),
                TurnStatuses = _state.Players
                    .ToDictionary(x => x.UserId, x => _state.PlayerStates[x.UserId].TurnStatus),
                DeadPlayers = _state.PlayerStates
                    .Where(x => x.Value.Statistics.DeathTurn != null)
                    .Select(x => x.Key)
                    .ToHashSet(),
                Turn = _state.Turn,
                TurnStatus = playerState.TurnStatus
            };
            return Task.FromResult<IRoomData>(data);
        }

        public Task SendMessage(Guid userId, TextMessageData textMessage)
        {
            EnsureUserIsOnline(userId);
            if (textMessage == null)
            {
                throw new ArgumentNullException(nameof(textMessage));
            }
            textMessage.EnsureIsValid<TextMessageData, TextMessageValidator>();
            var @event = new ChatMessageReceived(userId, textMessage);
            return _hub.NotifyEverybody(@event);
        }

        private Dictionary<Guid, PresenceStatus> GetPresenceStatuses()
        {
            return _state.Players.ToDictionary(
                x => x.UserId, 
                x => _hub.GetPresenceStatus(x.UserId));
        }

        public Task LaunchFleet(Guid userId, FleetData fleet)
        {
            if (fleet == null)
            {
                throw new ArgumentNullException(nameof(fleet));
            }
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);
            EnsureTurnIsInProgress(userId);

            var validator = new LaunchFleetValidator(userId, _map);
            validator.EnsureIsValid(fleet);
            LaunchFleetInternal(userId, fleet);
            return Task.CompletedTask;
        }

        public Task EndTurn(Guid userId, List<FleetData> fleets)
        {
            if (fleets == null)
            {
                throw new ArgumentNullException(nameof(fleets));
            }
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);
            EnsureTurnIsInProgress(userId);

            var validator = new LaunchFleetValidator(userId, _map);
            foreach (var fleet in fleets)
            {
                validator.EnsureIsValid(fleet);
                LaunchFleetInternal(userId, fleet);
            }
            return EndTurnInternal(userId);
        }

        public Task CancelFleet(Guid userId, FleetCancelationData fleetCancelation)
        {
            if (fleetCancelation == null)
            {
                throw new ArgumentNullException(nameof(fleetCancelation));
            }
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);
            EnsureTurnIsInProgress(userId);
            var waitingFleets = _state.PlayerStates[userId].WaitingFleets;
            var validator = new CancelFleetValidator(waitingFleets.Count);
            validator.EnsureIsValid(fleetCancelation);

            var fleet = waitingFleets[fleetCancelation.Index];
            waitingFleets.RemoveAt(fleetCancelation.Index);
            var planet = _map[fleet.From];
            planet.Ships += fleet.Ships;
            return Task.CompletedTask;
        }

        public Task EndTurn(Guid userId)
        {
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);

            return EndTurnInternal(userId);
        }

        public Task Start(Guid userId, InitialGameData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (_state.Status == GameStatus.Started)
            {
                return Task.CompletedTask;
            }

            _state.Status = GameStatus.Started;
            _state.Map = data.Map;
            _hub.Reset(data.HubMembers, data.HubJoinOrder);
            _state.Players = data.Players;
            _state.PlayerStates = data.Players
                .ToDictionary(x => x.UserId, x => new PlayerGameState());
            _state.Turn = 0;
            _map = new Map(_state.Map);
            if (_hub.LeaderUserId == null)
            {
                throw new InvalidOperationException($"Expected {nameof(_hub.LeaderUserId)} to be not null.");
            }
            var gameStarted = new GameStarted(
                _state.Players,
                GetPresenceStatuses(),
                _state.Map,
                _hub.LeaderUserId.Value);
            return _hub.NotifyEverybody(gameStarted);
        }

        public async Task<int> ProcessTick(int tick)
        {
            if (tick < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tick));
            }
            var nextTick = (tick + 1) % _options.TurnLengthInTicks;
            if (nextTick == 0)
            {
                return nextTick;
            }
            var ticked = new GameTicked(nextTick);
            await _hub.NotifyEverybody(ticked);
            return nextTick;
        }

        public async Task<IGameTurnOutcome> CalculateTurn()
        {
            EnsureGameIsInProgress();

            await _hub.NotifyEverybody(new TurnCalculationStarted(_state.Turn));
            CalculateResources();
            var activePlayers = _state.PlayerStates
                .Where(x => x.Value.Statistics.DeathTurn == null)
                .Shuffle()
                .Select(x => (x.Key, x.Value))
                .ToList();
            foreach (var (userId, playerState) in activePlayers)
            {
                EnqueueWaitingFleets(playerState);
                await MoveFleets(userId, playerState);
                playerState.TurnStatus = TurnStatus.Thinking;
            }
            await MarkDeadPlayers();
            var isEnded = await CheckGameEnd();
            var tasks = _state.PlayerStates
                .Select(x =>
                {
                    var turnCalculationEnded = new TurnCalculationEnded(
                        _state.Turn,
                        _state.Map,
                        x.Value.MovingFleets,
                        isEnded);
                    return _hub.Notify(x.Key, turnCalculationEnded);
                })
                .ToList();
            await Task.WhenAll(tasks);
            if (isEnded)
            {
                return new GameEndedTurnOutcome(_state.Turn);
            }
            if (IsInactive)
            {
                return new GameStalledTurnOutcome(EveryoneOfflinePeriod);
            }
            _state.Turn += 1;
            return new TurnCompletedTurnOutcome();
        }

        private void LaunchFleetInternal(Guid userId, FleetData fleet)
        {
            var state = _state.PlayerStates.GetOrCreateDefault(userId, () => new PlayerGameState());
            state.WaitingFleets.Add(fleet);
            var planet = _map[fleet.From];
            planet.Ships -= fleet.Ships;
        }

        private Task EndTurnInternal(Guid userId)
        {
            var state = _state.PlayerStates.GetOrCreateDefault(userId);
            state.TurnStatus = TurnStatus.Ended;
            return _hub.NotifyEverybody(new TurnEnded(userId));
        }

        private Task MarkDeadPlayers()
        {
            var events = _state.Players
                .Where(x => _state.PlayerStates[x.UserId].Statistics.DeathTurn == null)
                .Select(x => x.UserId)
                .Where(PlayerShouldDie)
                .Select(x => new PlayerDead(x))
                .ToList();
            foreach (var @event in events)
            {
                _state.PlayerStates[@event.UserId].TurnStatus = TurnStatus.Ended;
                _state.PlayerStates[@event.UserId].Statistics.DeathTurn = _state.Turn;
            }
            return _hub.NotifyEverybody(events);
        }

        private async Task<bool> CheckGameEnd()
        {
            var alivePlayerCount = _state.PlayerStates
                .Select(x => x.Value)
                .Count(x => x.Statistics.DeathTurn == null);
            if (alivePlayerCount > 1)
            {
                return false;
            }
            _state.Status = GameStatus.Ended;
            var @event = new GameEnded(
                _state.PlayerStates.ToDictionary(x => x.Key, x => x.Value.Statistics));
            await _hub.NotifyEverybody(@event);
            return true;
        }

        private bool PlayerShouldDie(Guid userId)
        {
            return _map.Planets.All(x => x.OwnerId != userId) &&
                   _state.PlayerStates[userId].MovingFleets.Count == 0;
        }

        private async Task MoveFleets(Guid userId, PlayerGameState player)
        {
            var arrived = new HashSet<MovingFleetData>();
            foreach (var movingFleet in player.MovingFleets)
            {
                movingFleet.Position = movingFleet.Route
                    .SkipWhile(x => x != movingFleet.Position)
                    .Skip(1)
                    .First();
                var destination = movingFleet.Fleet.To;
                if (movingFleet.Position != destination)
                {
                    continue;
                }
                arrived.Add(movingFleet);
                await HandleFleetArrival(userId, movingFleet.Fleet);
            }
            player.MovingFleets = player.MovingFleets
                .Where(x => !arrived.Contains(x))
                .ToList();
        }

        private async Task HandleFleetArrival(Guid senderUserId, FleetData fleet)
        {
            var attackerPlanet = _map[fleet.From];
            if (attackerPlanet.OwnerId == null)
            {
                throw new InvalidOperationException($"Attacker planet {attackerPlanet.Name} is neutral.");
            }
            var defenderPlanet = _map[fleet.To];
            if (senderUserId == defenderPlanet.OwnerId)
            {
                defenderPlanet.Ships += fleet.Ships;
                var reinforcementsArrived = new ReinforcementsArrived(
                    defenderPlanet.Name,
                    defenderPlanet.OwnerId.Value,
                    fleet.Ships);
                await _hub.Notify(senderUserId, reinforcementsArrived);
                return;
            }
            await HandleBattle(senderUserId, fleet, attackerPlanet, defenderPlanet);
        }

        private async Task HandleBattle(Guid attackerUserId, FleetData fleet, PlanetData attackerPlanet, PlanetData defenderPlanet)
        {
            if (attackerPlanet.OwnerId == null)
            {
                throw new InvalidOperationException($"Attacker planet {attackerPlanet.Name} is neutral.");
            }
            var attacker = new BattleFleet(fleet.Ships, attackerPlanet.Power);
            var defender = new BattleFleet(defenderPlanet.Ships, defenderPlanet.Power);
            var battleOutcome = _battleStrategy.Calculate(attacker, defender);
            var previousDefenderOwnerId = defenderPlanet.OwnerId;
            var attackerStats = _state.PlayerStates[attackerPlanet.OwnerId.Value].Statistics;
            attackerStats.ShipsDestroyed += defender.Ships - battleOutcome.DefenderShips;
            attackerStats.ShipsLost += attacker.Ships - battleOutcome.AttackerShips;
            if (battleOutcome.AttackerShips > 0)
            {
                attackerStats.BattlesWon += 1;
                defenderPlanet.OwnerId = attackerUserId;
                defenderPlanet.Ships = battleOutcome.AttackerShips;
            }
            else
            {
                attackerStats.BattlesLost += 1;
                defenderPlanet.Ships = battleOutcome.DefenderShips;
            }
            var attackOutcome = battleOutcome.AttackerShips > 0 ? AttackOutcome.Win : AttackOutcome.Defeat;
            var attackEnded = new AttackHappened(
                attackerUserId: attackerUserId,
                defenderUserId: previousDefenderOwnerId,
                planetName: defenderPlanet.Name,
                outcome: attackOutcome);
            if (previousDefenderOwnerId != null)
            {
                var defenderStats = _state.PlayerStates[previousDefenderOwnerId.Value].Statistics;
                defenderStats.ShipsDestroyed += attacker.Ships - battleOutcome.AttackerShips;
                defenderStats.ShipsLost += defender.Ships - battleOutcome.DefenderShips;
                if (attackOutcome == AttackOutcome.Win)
                {
                    defenderStats.BattlesLost += 1;
                }
                else
                {
                    defenderStats.BattlesWon += 1;
                }
            }
            await _hub.NotifyEverybody(attackEnded);
        }

        private void CalculateResources()
        {
            foreach (var planet in _map.Planets)
            {
                planet.Ships += planet.ProductionRate;
                if (planet.OwnerId != null)
                {
                    _state.PlayerStates[planet.OwnerId.Value].Statistics.ShipsProduced += planet.ProductionRate;
                }
            }
        }

        private void EnqueueWaitingFleets(PlayerGameState player)
        {
            foreach (var fleet in player.WaitingFleets)
            {
                var route = _map.GenerateRoute(fleet.From, fleet.To);
                var movingFleet = new MovingFleetData(fleet, route);
                player.MovingFleets.Add(movingFleet);
            }
            player.WaitingFleets.Clear();
        }

        private void EnsureGameIsInProgress()
        {
            if (_state.Status != GameStatus.Started)
            {
                throw new InvalidOperationException($"Game is ${_state.Status}.");
            }
        }

        private void EnsureUserIsOnline(Guid userId)
        {
            if (!_hub.IsOnline(userId))
            {
                throw new InvalidOperationException("Operation is only allowed for online members.");
            }
        }

        private void EnsureTurnIsInProgress(Guid userId)
        {
            var state = _state.PlayerStates.GetOrCreateDefault(userId);
            if (state.TurnStatus == TurnStatus.Ended)
            {
                throw UserException.Create(GameplayError.TurnIsAlreadyEnded, "You have already ended your turn.");
            }
        }
    }
}