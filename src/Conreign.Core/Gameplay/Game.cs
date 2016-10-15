using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Exceptions;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Errors;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Gameplay.Battle;
using Conreign.Core.Gameplay.Validators;
using Conreign.Core.Presence;
using Conreign.Core.Utility;

namespace Conreign.Core.Gameplay
{
    public class Game : IGame
    {
        private readonly GameState _state;
        private readonly IBattleStrategy _battleStrategy;
        private Hub _hub;
        private Map _map;

        public Game(GameState state, IBattleStrategy battleStrategy)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (battleStrategy == null)
            {
                throw new ArgumentNullException(nameof(battleStrategy));
            }
            _hub = new Hub(state.Hub);
            _map = new Map(state.Map);
            _state = state;
            _battleStrategy = battleStrategy;
        }

        public bool IsEnded => _state.IsEnded;
        public bool IsAnybodyThinking => _state.PlayerStates.Values.Any(x => x.TurnStatus == TurnStatus.Thinking);
        public int Turn => _state.Turn;

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
                Self = data.Hub,
                Members = data.HubMembers,
                JoinOrder = data.HubJoinOrder
            };
            _state.Players = data.Players;
            _state.PlayerStates = data.Players.ToDictionary(x => x.UserId, x => new PlayerGameState());
            _state.Turn = 0;
            _map = new Map(_state.Map);
            _hub = new Hub(_state.Hub);
            return Task.CompletedTask;
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);
            var playerState = _state.PlayerStates.GetOrCreateDefault(userId);
            var data = new GameData
            {
                Players = _state.Players,
                Events = _hub.GetEvents(userId).ToList(),
                LeaderUserId = _hub.LeaderUserId,
                Map = _state.Map,
                MovingFleets = playerState.MovingFleets,
                WaitingFleets = playerState.WaitingFleets,
                PlayerStatuses = _state.Players
                    .ToDictionary(x => x.UserId, x => GetPresenceStatus(x.UserId)),
                DeadPlayers = _state.PlayerStates
                    .Where(x => x.Value.Statistics.DeathTurn != null)
                    .Select(x => x.Key)
                    .ToHashSet(),
                Turn = _state.Turn,
                TurnStatus = playerState.TurnStatus
            };
            return Task.FromResult<IRoomData>(data);
        }

        private PresenceStatus GetPresenceStatus(Guid userId)
        {
            return _hub.HasMemberOnline(userId) ? PresenceStatus.Online : PresenceStatus.Offline;
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
            var state = _state.PlayerStates.GetOrCreateDefault(userId, () => new PlayerGameState());;
            state.WaitingFleets.Add(fleet);
            var planet = _map.GetPlanetByNameOrNull(fleet.From);
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
            waitingFleets.RemoveAt(fleetCancelation.Index);
            return Task.CompletedTask;
        }

        public Task EndTurn(Guid userId)
        {
            EnsureGameIsInProgress();
            EnsureUserIsOnline(userId);

            var state = _state.PlayerStates.GetOrCreateDefault(userId);
            state.TurnStatus = TurnStatus.Ended;
            return _hub.NotifyEverybody(new TurnEnded(userId));
        }

        public async Task CalculateTurn()
        {
            EnsureGameIsInProgress();

            await NotifyEverybody(new TurnCalculationStarted(_state.Turn));
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
            var tasks = _state.PlayerStates
                .Select(x =>
                {
                    var turnCalculationEnded = new TurnCalculationEnded(
                        turn: _state.Turn,
                        map: _state.Map,
                        movingFleets: x.Value.MovingFleets);
                    return this.Notify(x.Key, turnCalculationEnded);
                })
                .ToList();
            await Task.WhenAll(tasks);
            await MarkDeadPlayers();
            await CheckGameEnd();
            _state.Turn += 1;
        }

        public Task Notify(ISet<Guid> users, params IEvent[] events)
        {
            return _hub.Notify(users, events);
        }

        public Task NotifyEverybody(params IEvent[] events)
        {
            return _hub.NotifyEverybody(events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            return _hub.NotifyEverybodyExcept(users, events);
        }

        public Task Join(Guid userId, IPublisher<IEvent> publisher)
        {
            return _hub.Join(userId, publisher);
        }

        public Task Leave(Guid userId)
        {
            return _hub.Leave(userId);
        }

        private Task MarkDeadPlayers()
        {
            var events = _state.Players
                .Select(x => x.UserId)
                .Where(PlayerShouldDie)
                .Select(x => new PlayerDead(x))
                .ToList();
            foreach (var @event in events)
            {
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
            return this.NotifyEverybody(@event);
        }

        private bool PlayerShouldDie(Guid userId)
        {
            return _map.All(x => x.OwnerId != userId) && _state.PlayerStates[userId].MovingFleets.Count == 0;
        }

        private async Task MoveFleets(PlayerGameState player)
        {
            var arrived = new HashSet<MovingFleetData>();
            foreach (var movingFleet in player.MovingFleets)
            {
                movingFleet.Position = movingFleet.Route
                    .SkipWhile(x => x != movingFleet.Position)
                    .First();
                var destination = _map.GetPlanetPositionByName(movingFleet.Fleet.To);
                if (movingFleet.Position != destination)
                {
                    continue;
                }
                arrived.Add(movingFleet);
                // Handle battle
                await HandleBattle(movingFleet);
            }
            player.MovingFleets = player.MovingFleets
                .Where(x => !arrived.Contains(x))
                .ToList();
        }

        private async Task HandleBattle(MovingFleetData movingFleet)
        {
            var attackPlanet = _map.GetPlanetByNameOrNull(movingFleet.Fleet.From);
            if (attackPlanet.OwnerId == null)
            {
                throw new InvalidOperationException($"Attack planet {attackPlanet.Name} does not have an owner.");
            }
            var attacker = new BattleFleet(movingFleet.Fleet.Ships, attackPlanet.Power);
            var defenderPlanet = _map.GetPlanetByNameOrNull(movingFleet.Fleet.To);
            var defender = new BattleFleet(defenderPlanet.Ships, defenderPlanet.Power);
            var battleOutcome = _battleStrategy.Calculate(attacker, defender);
            var previousDefenderOwnerId = defenderPlanet.OwnerId;
            var attackerStats = _state.PlayerStates[attackPlanet.OwnerId.Value].Statistics;
            attackerStats.ShipsDestroyed += defender.Ships - battleOutcome.DefenderShips;
            attackerStats.ShipsLost += attacker.Ships - battleOutcome.AttackerShips;
            if (battleOutcome.AttackerShips > 0)
            {
                attackerStats.BattlesWon += 1;
                defenderPlanet.OwnerId = attackPlanet.OwnerId;
                defenderPlanet.Ships = battleOutcome.AttackerShips;
            }
            else
            {
                attackerStats.BattlesLost += 1;
                defenderPlanet.Ships = battleOutcome.DefenderShips;
            }
            var attackOutcome = battleOutcome.AttackerShips > 0 ? AttackOutcome.Win : AttackOutcome.Defeat;
            var attackEnded = new AttackEnded(
                attackerUserId: attackPlanet.OwnerId.Value,
                defenderUserId: defenderPlanet.OwnerId,
                planetName: defenderPlanet.Name,
                outcome: attackOutcome);
            await this.Notify(attackPlanet.OwnerId.Value, attackEnded);
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
                await this.Notify(previousDefenderOwnerId.Value, attackEnded);
            }
        }

        private void CalculateResources()
        {
            foreach (var planet in _map)
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
                var position = _map.GetPlanetPositionByName(fleet.From);
                var movingFleet = new MovingFleetData
                {
                    Route = _map.CalculateRoute(fleet.From, fleet.To),
                    Fleet = fleet,
                    Position = position
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
            if (!_hub.HasMemberOnline(userId))
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
