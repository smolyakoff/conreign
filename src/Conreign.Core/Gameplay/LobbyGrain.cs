using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Commands;
using Orleans;
using Orleans.Placement;

namespace Conreign.Core.Gameplay
{
    [PreferLocalPlacement]
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private Lobby _lobby;

        public override Task OnActivateAsync()
        {
            _lobby = new Lobby(State, this);
            return Task.CompletedTask;
        }

        public Task Join(JoinCommand command)
        {
            return _lobby.Join(command);
        }

        public Task Leave(LeaveCommand command)
        {
            return _lobby.Leave(command);
        }

        public Task Notify(NotifyCommand command)
        {
            return _lobby.Notify(command);
        }

        public Task NotifyEverybody(NotifyEverybodyCommand command)
        {
            return _lobby.NotifyEverybody(command);
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateGameSettings()
        {
            throw new System.NotImplementedException();
        }

        public Task UpdatePlayerOptions(UpdatePlayerOptionsCommand command)
        {
            throw new System.NotImplementedException();
        }

        public Task<IGame> StartGame(StartGameCommand command)
        {
            return _lobby.StartGame(command);
        }

        public async Task<IGame> CreateGame()
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitializeGameCommand(
                State.Map,
                State.Players,
                State.Hub.Members
            );
            await game.Initialize(command);
            DeactivateOnIdle();
            return game;
        }
    }
}
