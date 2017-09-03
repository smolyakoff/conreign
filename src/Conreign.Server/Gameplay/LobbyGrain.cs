using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Gameplay
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

        public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            await _lobby.UpdateGameOptions(userId, options);
            await WriteStateAsync();
        }

        public async Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            await _lobby.UpdatePlayerOptions(userId, options);
            await WriteStateAsync();
        }

        public async Task<IGame> StartGame(Guid userId)
        {
            var game = await _lobby.StartGame(userId);
            return game;
        }

        public async Task<IGame> CreateGame(Guid userId)
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitialGameData(
                userId,
                State.MapEditor.Map,
                State.Players,
                State.Hub.Members.ToDictionary(x => x.Key, x => x.Value.ConnectionIds),
                State.Hub.JoinOrder
            );
            await game.Initialize(command);
            return game;
        }

        public async Task Handle(GameEnded @event)
        {
            await _lobby.Handle(@event);
            DeactivateOnIdle();
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
            await ClearStateAsync();
            await base.OnDeactivateAsync();
        }

        private void InitializeState()
        {
            State.RoomId = this.GetPrimaryKeyString();
            State.Hub.Id = State.RoomId;
        }
    }
}