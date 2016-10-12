using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
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
        private IBus _playerBus;

        public override async Task OnActivateAsync()
        {
            await InitializeState();
            _publisher = GrainFactory.GetGrain<IClientPublisherGrain>(State.UserId, State.RoomId, null);
            _player = new Player(State, _publisher);
            _roomBus = GrainFactory.GetGrain<IBusGrain>(SystemTopics.Room(State.RoomId)).AsBus();
            _playerBus = GrainFactory.GetGrain<IBusGrain>(SystemTopics.Player(State.UserId, State.RoomId)).AsBus();
            await _roomBus.Subscribe<GameStarted.System>(this.AsReference<IPlayerGrain>());
            await _playerBus.Subscribe<Connected>(this.AsReference<IPlayerGrain>());
            await _playerBus.Subscribe<Disconnected>(this.AsReference<IPlayerGrain>());
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _roomBus.UnsubscribeAll(this.AsReference<IPlayerGrain>());
            await _playerBus.UnsubscribeAll(this.AsReference<IPlayerGrain>());
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

        private Task InitializeState()
        {
            string roomId;
            State.UserId = this.GetPrimaryKey(out roomId);
            State.RoomId = roomId;
            if (State.Room == null)
            {
                State.Room = GrainFactory.GetGrain<ILobbyGrain>(roomId);
            }
            return Task.CompletedTask;
        }

        public Task Handle(GameStarted.System @event)
        {
            return _player.Handle(@event);
        }

        public async Task Handle(Connected @event)
        {
            await _publisher.Handle(@event);
            await _player.Handle(@event);
        }

        public async Task Handle(Disconnected @event)
        {
            await _publisher.Handle(@event);
            await _player.Handle(@event);
        }
    }
}