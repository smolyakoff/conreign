using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Commands;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class GameGrain : Grain<GameState>, IGameGrain
    {
        private Game _game;

        public override Task OnActivateAsync()
        {
            _game = new Game(State);
            return base.OnActivateAsync();
        }

        public Task Join(JoinCommand command)
        {
            return _game.Join(command);
        }

        public Task Leave(LeaveCommand command)
        {
            return _game.Leave(command);
        }

        public Task Notify(NotifyCommand command)
        {
            return _game.Notify(command);
        }

        public Task NotifyEverybody(NotifyEverybodyCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public Task LaunchFleet()
        {
            throw new System.NotImplementedException();
        }

        public Task EndTurn()
        {
            throw new System.NotImplementedException();
        }

        public Task Initialize(InitializeGameCommand command)
        {
            State.Map = command.Map;
            State.Hub.Members = command.HubMembers;
            State.Players = command.Players;
            return Task.CompletedTask;
        }
    }
}
