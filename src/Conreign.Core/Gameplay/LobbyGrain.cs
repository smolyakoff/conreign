using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private Lobby _lobby;
        private IBusGrain _bus;

        public override async Task OnActivateAsync()
        {
            await InitializeState();
            _lobby = new Lobby(State, this);
            _bus = GrainFactory.GetGrain<IBusGrain>(ServerTopics.Room(State.RoomId));
            await _bus.Subscribe(this.AsReference<ILobbyGrain>());
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _bus.Unsubscribe(this.AsReference<ILobbyGrain>());
            await base.OnDeactivateAsync();
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            return _lobby.GetState(userId);
        }

        public Task Notify(ISet<Guid> users, params IEvent[] @event)
        {
            return _lobby.Notify(users, @event);
        }

        public Task NotifyEverybody(params IEvent[] @event)
        {
            return _lobby.NotifyEverybody(@event);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            return _lobby.NotifyEverybodyExcept(users, events);
        }

        public Task Join(Guid userId, IPublisher<IEvent> publisher)
        {
            return _lobby.Join(userId, publisher);
        }

        public Task Leave(Guid userId)
        {
            return _lobby.Leave(userId);
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

        private Task InitializeState()
        {
            State.RoomId = this.GetPrimaryKeyString();
            State.Hub.Self = GrainFactory.GetGrain<IBusGrain>(ServerTopics.Room(State.RoomId));
            return Task.CompletedTask;
        }

        public async Task<IGame> CreateGame()
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitialGameData(
                map: State.MapEditor.Map,
                players: State.Players,
                hub: State.Hub.Self,
                hubMembers: State.Hub.Members,
                hubJoinOrder: State.Hub.JoinOrder
            );
            await game.Initialize(command);
            return game;
        }

        public Task Handle(GameEnded @event)
        {
            return _lobby.Handle(@event);
        }
    }
}
