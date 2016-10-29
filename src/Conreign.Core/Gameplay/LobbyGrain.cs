using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Gameplay
{
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private Lobby _lobby;
        private StreamSubscriptionHandle<IServerEvent> _subscription;

        public Task<IRoomData> GetState(Guid userId)
        {
            return _lobby.GetState(userId);
        }

        public Task Notify(ISet<Guid> userIds, params IEvent[] @event)
        {
            return _lobby.Notify(userIds, @event);
        }

        public Task NotifyEverybody(params IEvent[] @event)
        {
            return _lobby.NotifyEverybody(@event);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
        {
            return _lobby.NotifyEverybodyExcept(userIds, events);
        }

        public Task Connect(Guid userId, Guid connectionId)
        {
            return _lobby.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _lobby.Disconnect(userId, connectionId);
        }

        public Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            return _lobby.UpdateGameOptions(userId, options);
        }

        public Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            return _lobby.UpdatePlayerOptions(userId, options);
        }

        public Task GenerateMap(Guid userId)
        {
            return _lobby.GenerateMap(userId);
        }

        public async Task<IGame> StartGame(Guid userId)
        {
            var game = await _lobby.StartGame(userId);
            // DeactivateOnIdle();
            return game;
        }

        public async Task<IGame> CreateGame(Guid userId)
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitialGameData(
                initiatorId: userId,
                map: State.MapEditor.Map,
                players: State.Players,
                hubMembers: State.Hub.Members.ToDictionary(x => x.Key, x => x.Value.ConnectionIds),
                hubJoinOrder: State.Hub.JoinOrder
                );
            await game.Initialize(command);
            return game;
        }

        public Task Handle(GameEnded @event)
        {
            return _lobby.Handle(@event);
        }

        public override async Task OnActivateAsync()
        {
            InitializeState();
            var topic = Topic.Room(GetStreamProvider(StreamConstants.ProviderName), this.GetPrimaryKeyString());
            _lobby = new Lobby(State, topic, this);
            _subscription = await topic.EnsureIsSubscribedOnce(this);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync();
        }

        private void InitializeState()
        {
            State.RoomId = this.GetPrimaryKeyString();
        }
    }
}