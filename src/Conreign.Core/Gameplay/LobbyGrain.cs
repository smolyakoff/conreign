using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private Lobby _lobby;

        public override Task OnActivateAsync()
        {
            _lobby = new Lobby(State, this.AsReference<ILobbyGrain>());
            return Task.CompletedTask;
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateGameOptions()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IGame> CreateGame()
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new GameData(
                State.Map,
                State.Players,
                State.Hub.Members
            );
            await game.Initialize(command);
            DeactivateOnIdle();
            return game;
        }

        public Task Notify(object @event, ISet<Guid> users)
        {
            return _lobby.Notify(@event, users);
        }

        public Task NotifyEverybody(object @event)
        {
            return _lobby.NotifyEverybody(@event);
        }

        public Task Join(Guid userId, IObserver observer)
        {
            return _lobby.Join(userId, observer);
        }

        public Task Leave(Guid userId)
        {
            return _lobby.Leave(userId);
        }

        public Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task<IGame> StartGame(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task Notify(object @event)
        {
            throw new NotImplementedException();
        }
    }
}
