using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Errors;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Contracts.Presence;
using Conreign.Core;
using Conreign.Core.Battle;
using Conreign.Core.Utility;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Gameplay.Validators;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    public class Game : IGame
    {
        private readonly IBattleStrategy _battleStrategy;
        private readonly GameState _state;
        private readonly IUserTopic _topic;
        private Hub _hub;
        private Map _map;

        public Game(GameState state, IUserTopic topic, IBattleStrategy battleStrategy)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _hub = new Hub(_state.Hub, topic);
            _map = new Map(_state.Map);
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            _battleStrategy = battleStrategy ?? throw new ArgumentNullException(nameof(battleStrategy));
        }

        public bool IsOnlinePlayersThinking => _state.PlayerStates
            .Where(kv => _hub.IsOnline(kv.Key))
            .Select(kv => kv.Value)
            .Any(p => p.TurnStatus == TurnStatus.Thinking);

        public int Turn => _state.Turn;

        public TimeSpan EveryoneOfflinePeriod => _hub.EveryoneOfflinePeriod;

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
                RoomId = _state.RoomId,
                Players = _state.Players,
                Events = _hub.GetEvents(userId).Select(x => x.ToEnvelope()).ToList(),
                LeaderUserId = _hub.LeaderUserId,
                Map = _state.Map,
                MovingFleets = playerState.MovingFleets,
                WaitingFleets = playerState.WaitingFleets,
                PresenceStatuses = _state.Players
                    .ToDictionary(x => x.UserId, x => GetPresenceStatus(x.UserId)),
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
            fleet.EnsureIsValid(validator);
            var state = _state.PlayerStates.GetOrCreateDefault(userId, () => new PlayerGameState());
            state.WaitingFleets.Add(fleet);
            var planet = _map[fleet.From];
            planet.Ships -= fleet.Ships;
            return Task.CompletedTask;
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
            fleetCancelation.EnsureIsValid(validator);

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

            var state = _state.PlayerStates.GetOrCreateDefault(userId);
            state.TurnStatus = TurnStatus.Ended;
            return _hub.NotifyEverybody(new TurnEnded(_state.RoomId, userId));
        }

        public Task Notify(ISet<Guid> userIds, params IEvent[] events)
        {
            return _hub.Notify(userIds, events);
        }

        public Task NotifyEverybody(params IEvent[] events)
        {
            return _hub.NotifyEverybody(events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
        {
            return _hub.NotifyEverybodyExcept(userIds, events);
        }

        public Task Initialize(InitialGameData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (_state.IsStarted)
            {
                throw new InvalidOperationException("Expected game to be not started.");
            }
            _state.IsEnded = false;
            _state.IsStarted = true;
            _state.Map = data.Map;
            _state.Hub = new HubState
            {
                Id = _state.RoomId,
                Members = data.HubMembers
                    .ToDictionary(x => x.Key, x => new HubMemberState {ConnectionIds = x.Value}),
                JoinOrder = data.HubJoinOrder
            };
            _state.Players = data.Players;
            _state.PlayerStates = data.Players
                .ToDictionary(x => x.UserId, x => new PlayerGameState());
            _state.Turn = 0;
            _map = new Map(_state.Map);
            _hub = new Hub(_state.Hub, _topic);
            return Task.CompletedTask;
        }

        public async Task<bool> CalculateTurn()
        {
            EnsureGameIsInProgress();

            await NotifyEverybody(new TurnCalculationStarted(_state.RoomId, _state.Turn));
            CalculateResources();
            var activePlayers = _state.PlayerStates.Values
                .Where(x => x.Statistics.DeathTurn == null)
                .Shuffle()
                .ToList();
            foreach (var player in activePlayers)
            {
                EnqueueWaitingFleets(player);
                await MoveFleets(player);
                player.TurnStatus = TurnStatus.Thinking;
            }
            await MarkDeadPlayers();
            await CheckGameEnd();
            var tasks = _state.PlayerStates
                .Select(x =>
                {
                    var turnCalculationEnded = new TurnCalculationEnded(
                        _state.RoomId,
                        _state.Turn,
                        _state.Map,
                        x.Value.MovingFleets);
                    return this.Notify(x.Key, turnCalculationEnded);
                })
                .ToList();
            await Task.WhenAll(tasks);
            if (_state.IsEnded)
            {
                return true;
            }
            _state.Turn += 1;
            return false;
        }

        private PresenceStatus GetPresenceStatus(Guid userId)
        {
            return _hub.IsOnline(userId) ? PresenceStatus.Online : PresenceStatus.Offline;
        }

        private Task MarkDeadPlayers()
        {
            var events = _state.Players
                .Where(x => _state.PlayerStates[x.UserId].Statistics.DeathTurn == null)
                .Select(x => x.UserId)
                .Where(PlayerShouldDie)
                .Select(x => new PlayerDead(_state.RoomId, x))
                .ToList();
            foreach (var @event in events)
            {
                _state.PlayerStates[@event.UserId].TurnStatus = TurnStatus.Ended;
                _state.PlayerStates[@event.UserId].Statistics.DeathTurn = _state.Turn;
            }
            return this.NotifyEverybody(events);
        }

        private Task CheckGameEnd()
        {
            var alivePlayerCount = _state.PlayerStates
                .Select(x => x.Value)
                .Count(x => x.Statistics.DeathTurn == null);
            if (alivePlayerCount > 1)
            {
                return Task.CompletedTask;
            }
            _state.IsEnded = true;
            _state.IsStarted = false;
            var @event = new GameEnded(
                _state.RoomId,
                _state.PlayerStates.ToDictionary(x => x.Key, x => x.Value.Statistics));
            return NotifyEverybody(@event);
        }

        private bool PlayerShouldDie(Guid userId)
        {
            return _map.Planets.All(x => x.OwnerId != userId) &&
                   _state.PlayerStates[userId].MovingFleets.Count == 0;
        }

        private async Task MoveFleets(PlayerGameState player)
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
                await HandleFleetArrival(movingFleet.Fleet);
            }
            player.MovingFleets = player.MovingFleets
                .Where(x => !arrived.Contains(x))
                .ToList();
        }

        private async Task HandleFleetArrival(FleetData fleet)
        {
            var attackerPlanet = _map[fleet.From];
            if (attackerPlanet.OwnerId == null)
            {
                throw new InvalidOperationException($"Attacker planet {attackerPlanet.Name} is neutral.");
            }
            var defenderPlanet = _map[fleet.To];
            if (attackerPlanet.OwnerId == defenderPlanet.OwnerId)
            {
                defenderPlanet.Ships += fleet.Ships;
                var reinforcementsArrived = new ReinforcementsArrived(
                    _state.RoomId,
                    attackerPlanet.Name,
                    attackerPlanet.OwnerId.Value,
                    fleet.Ships);
                await this.Notify(attackerPlanet.OwnerId.Value, reinforcementsArrived);
                return;
            }
            await HandleBattle(fleet, attackerPlanet, defenderPlanet);
        }

        private async Task HandleBattle(FleetData fleet, PlanetData attackerPlanet, PlanetData defenderPlanet)
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
                defenderPlanet.OwnerId = attackerPlanet.OwnerId;
                defenderPlanet.Ships = battleOutcome.AttackerShips;
            }
            else
            {
                attackerStats.BattlesLost += 1;
                defenderPlanet.Ships = battleOutcome.DefenderShips;
            }
            var attackOutcome = battleOutcome.AttackerShips > 0 ? AttackOutcome.Win : AttackOutcome.Defeat;
            var attackEnded = new AttackHappened(
                _state.RoomId,
                attackerUserId: attackerPlanet.OwnerId.Value,
                defenderUserId: defenderPlanet.OwnerId,
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
            await NotifyEverybody(attackEnded);
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
                var movingFleet = new MovingFleetData
                {
                    Route = _map.GenerateRoute(fleet.From, fleet.To),
                    Fleet = fleet,
                    Position = fleet.From
                };
                player.MovingFleets.Add(movingFleet);
            }
            player.WaitingFleets.Clear();
        }

        private void EnsureGameIsInProgress()
        {
            if (_state.IsEnded)
            {
                throw new InvalidOperationException("Game has already ended.");
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