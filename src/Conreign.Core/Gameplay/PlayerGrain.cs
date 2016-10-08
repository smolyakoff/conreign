using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private Player _player;
        private IClientObserverGrain _observer;

        public override Task OnActivateAsync()
        {
            string roomId;
            var userId = this.GetPrimaryKey(out roomId);
            _observer = GrainFactory.GetGrain<IClientObserverGrain>(userId, roomId, null);
            InitializeState();
            _player = new Player(
                State, 
                _observer);
            return base.OnActivateAsync();
        }

        public Task UpdateOptions(PlayerOptionsData options)
        {
            return _player.UpdateOptions(options);
        }

        public Task UpdateGameOptions(GameOptionsData options)
        {
            return _player.UpdateGameOptions(options);
        }

        public Task StartGame()
        {
            return _player.StartGame();
        }

        public Task LaunchFleet(FleetData fleet)
        {
            return _player.LaunchFleet(fleet);
        }

        public Task EndTurn()
        {
            return _player.EndTurn();
        }

        public Task Write(string text)
        {
            return _player.Write(text);
        }

        public Task<IRoomData> GetState()
        {
            return _player.GetState();
        }

        public async Task Connect(Guid connectionId)
        {
            await _observer.Connect(connectionId);
            await _player.Connect(connectionId);
        }

        public async Task Disconnect(Guid connectionId)
        {
            await _observer.Disconnect(connectionId);
            await _player.Disconnect(connectionId);
        }

        private void InitializeState()
        {
            string roomId;
            State.UserId = this.GetPrimaryKey(out roomId);
            State.RoomId = roomId;
            if (State.Room == null)
            {
                State.Room = GrainFactory.GetGrain<ILobbyGrain>(roomId);
            }
            foreach (var connectionId in State.ConnectionIds)
            {
                _observer.Connect(connectionId);
            }
        }
    }
}