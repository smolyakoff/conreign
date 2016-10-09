using System;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private Player _player;
        private IClientPublisherGrain _publisher;
        private IBus _roomBus;

        public override async Task OnActivateAsync()
        {
            InitializeStateAndPublisher();
            _player = new Player(State, _publisher);
            _roomBus = GrainFactory.GetGrain<IBusGrain>(State.RoomId).AsBus();
            await _roomBus.Subscribe(this.AsReference<IPlayerGrain>());
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _roomBus.Unsubscribe(this.AsReference<IPlayerGrain>());
            await base.OnDeactivateAsync();
        }

        public Task UpdateOptions(PlayerOptionsData options)
        {
            return _player.UpdateOptions(options);
        }

        public Task UpdateGameOptions(GameOptionsData options)
        {
            return _player.UpdateGameOptions(options);
        }

        public async Task StartGame()
        {
            await _player.StartGame();
            await WriteStateAsync();
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
            await _publisher.Connect(connectionId);
            await _player.Connect(connectionId);
        }

        public async Task Disconnect(Guid connectionId)
        {
            await _publisher.Disconnect(connectionId);
            await _player.Disconnect(connectionId);
            if (State.ConnectionIds.Count == 0)
            {
                DeactivateOnIdle();
            }
        }

        private void InitializeStateAndPublisher()
        {
            string roomId;
            State.UserId = this.GetPrimaryKey(out roomId);
            State.RoomId = roomId;
            if (State.Room == null)
            {
                State.Room = GrainFactory.GetGrain<ILobbyGrain>(roomId);
            }
            _publisher = GrainFactory.GetGrain<IClientPublisherGrain>(State.UserId, roomId, null);
            foreach (var connectionId in State.ConnectionIds)
            {
                _publisher.Connect(connectionId);
            }
        }

        public Task Handle(GameStarted.System @event)
        {
            return _player.Handle(@event);
        }
    }
}