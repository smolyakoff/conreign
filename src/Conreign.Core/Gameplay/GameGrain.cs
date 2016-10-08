using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
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

        public Task Initialize(GameData data)
        {
            State.Map = data.Map;
            State.Hub.Members = data.HubMembers;
            State.Players = data.Players.Select(p => new GamePlayerState
            {
                Color = p.Color,
                Nickname = p.Nickname,
                UserId = p.UserId
            }).ToList();
            return Task.CompletedTask;
        }

        public Task<IRoomState> GetState(Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public Task LaunchFleet()
        {
            throw new System.NotImplementedException();
        }

        public Task LaunchFleet(Guid userId, FleetData fleet)
        {
            throw new NotImplementedException();
        }

        public Task EndTurn()
        {
            throw new System.NotImplementedException();
        }

        public Task Notify(object @event, ISet<Guid> users)
        {
            return _game.Notify(@event, users);
        }

        public Task NotifyEverybody(object @event)
        {
            return _game.NotifyEverybody(@event);
        }

        public Task Join(Guid userId, IObserver observer)
        {
            return _game.Join(userId, observer);
        }

        public Task Leave(Guid userId)
        {
            return _game.Leave(userId);
        }
    }
}
