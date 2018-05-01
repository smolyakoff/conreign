using System;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Gameplay
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private Player _player;
        private StreamSubscriptionHandle<IServerEvent> _subscription;

        private IRoom Room
        {
            get
            {
                if (State.RoomMode == RoomMode.Lobby)
                {
                    return GrainFactory.GetGrain<ILobbyGrain>(State.RoomId);
                }
                return GrainFactory.GetGrain<IGameGrain>(State.RoomId);
            }
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

        public Task CancelFleet(FleetCancelationData fleetCancelation)
        {
            return _player.CancelFleet(fleetCancelation);
        }

        public Task EndTurn()
        {
            return _player.EndTurn();
        }

        public Task SendMessage(TextMessageData textMessage)
        {
            return _player.SendMessage(textMessage);
        }

        public async Task<IRoomData> GetState()
        {
            return await Room.GetState(State.UserId);
        }

        public Task Handle(GameStarted @event)
        {
            return _player.Handle(@event);
        }

        public async Task Handle(GameEnded @event)
        {
            await _player.Handle(@event);
            await WriteStateAsync();
        }

        public override async Task OnActivateAsync()
        {
            InitializeState();
            var provider = GetStreamProvider(StreamConstants.ProviderName);
            var stream = provider.GetServerStream(TopicIds.Player(State.UserId, State.RoomId));
            _player = new Player(State, () => Room);
            _subscription = await this.EnsureIsSubscribedOnce(stream);
        }

        public override Task OnDeactivateAsync()
        {
            return _subscription.UnsubscribeAsync();
        }

        private void InitializeState()
        {
            State.UserId = this.GetPrimaryKey(out string roomId);
            State.RoomId = roomId;
            State.RoomMode = RoomMode.Lobby;
        }

        public Task Connect(Guid connectionId)
        {
            return _player.Connect(connectionId);
        }

        public Task Disconnect(Guid connectionId)
        {
            return _player.Disconnect(connectionId);
        }
    }
}