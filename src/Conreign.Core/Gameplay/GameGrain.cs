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

        public Task<IRoomData> GetState(Guid userId)
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

        public Task EndTurn(Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public Task Notify(ISet<Guid> users, params IClientEvent[] events)
        {
            return _game.Notify(users, events);
        }

        public Task NotifyEverybody(params IClientEvent[] events)
        {
            return _game.NotifyEverybody(events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IClientEvent[] events)
        {
            return _game.NotifyEverybodyExcept(users, events);
        }

        public Task Join(Guid userId, IClientObserver observer)
        {
            return _game.Join(userId, observer);
        }

        public Task Leave(Guid userId)
        {
            return _game.Leave(userId);
        }
    }
}
