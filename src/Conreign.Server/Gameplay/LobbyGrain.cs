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
using Serilog;

namespace Conreign.Server.Gameplay
{
    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private readonly LobbyGrainOptions _options;
        private Lobby _lobby;
        private StreamSubscriptionHandle<IServerEvent> _subscription;
        private ILogger _logger;
        private IDisposable _inactivityTimer;

        private string RoomId => this.GetPrimaryKeyString();

        public LobbyGrain(ILogger logger, LobbyGrainOptions options)
        {
            _logger = logger.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IRoomData> GetState(Guid userId)
        {
            var state = await _lobby.GetState(userId);
            state.RoomId = RoomId;
            return state;
        }

        public Task SendMessage(Guid userId, TextMessageData textMessage)
        {
            return _lobby.SendMessage(userId, textMessage);
        }

        public Task Connect(Guid userId, Guid connectionId)
        {
            return _lobby.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _lobby.Disconnect(userId, connectionId);
        }

        public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            await _lobby.UpdateGameOptions(userId, options);
            await WriteStateAsync();
        }

        public async Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            await _lobby.UpdatePlayerOptions(userId, options);
            await WriteStateAsync();
        }

        public async Task<InitialGameData> InitializeGame(Guid userId)
        {
            var data = await _lobby.InitializeGame(userId);
            await WriteStateAsync();
            return data;
        }

        public Task Handle(GameEnded @event)
        {
            return _lobby.Handle(@event);
        }

        public override async Task OnActivateAsync()
        {
            /*
             * It's not possible to initialize the logger in constructor as Orleans throws an exception
             * when primary key is accessed before grain activation
             */
            _logger = _logger.ForContext(nameof(RoomId), RoomId);
            var topic = Topic.Room(GetStreamProvider(StreamConstants.ProviderName), RoomId);
            _logger.Information("Lobby [{RoomId}] is activated.");
            _lobby = new Lobby(State, topic);
            _subscription = await topic.EnsureIsSubscribedOnce(this);
            var inactivityTimerInterval = TimeSpan.FromTicks(_options.MaxInactivityPeriod.Ticks / 2);
            _inactivityTimer = RegisterTimer(
                EnsureSomeoneIsOnline,
                null,
                inactivityTimerInterval,
                inactivityTimerInterval);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _subscription.UnsubscribeAsync();
            _inactivityTimer?.Dispose();
            await ClearStateAsync();
            await base.OnDeactivateAsync();
        }

        private Task EnsureSomeoneIsOnline(object state)
        {
            if (_lobby.EveryoneOfflinePeriod > _options.MaxInactivityPeriod)
            {
                _logger.Information(
                    "Going to deactivate lobby due to inactivity. Inactivity period was {InactivityPeriod}.",
                    _lobby.EveryoneOfflinePeriod);
                _inactivityTimer?.Dispose();
                _inactivityTimer = null;
                DeactivateOnIdle();
            }

            return Task.CompletedTask;
        }
    }
}