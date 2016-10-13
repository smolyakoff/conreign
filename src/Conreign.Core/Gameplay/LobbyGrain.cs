using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain, IGameFactory
    {
        private Lobby _lobby;

        public override async Task OnActivateAsync()
        {
            await InitializeState();
            _lobby = new Lobby(State, this);
            await base.OnActivateAsync();
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
            throw new NotImplementedException();
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
            return Task.CompletedTask;
        }

        public async Task<IGame> CreateGame()
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitialGameData(
                State.MapEditor.Map,
                State.Players,
                State.Hub.Members
            );
            await game.Initialize(command);
            return game;
        }
    }
}
