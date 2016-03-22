using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Auth;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Game.Data;
using Conreign.Core.Utility;
using Orleans;

namespace Conreign.Core.Game
{
    public class PlayerGrain : Grain<PlayerData>, IPlayerGrain
    {
        private static readonly TimeSpan StateUpdateTimeout = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan StateUpdateInterval = TimeSpan.FromMinutes(1);

        private bool _connectionsUpdated;

        private IDisposable _stateUpdateTimer;

        static PlayerGrain()
        {
            var configuration = new MapperConfiguration(cfg =>
                cfg.CreateMap<PlayerSettingsPayload, PlayerSettingsData>().ReverseMap());
            Mapper = configuration.CreateMapper();
        }

        private static IMapper Mapper { get; }

        public Task Connect(ConnectAction action)
        {
            action.EnsureNotNull().EnsureAuthorized(action.Meta.User.UserKey == this.GetPrimaryKey());

            var connection = new ConnectionData(action.Payload.ConnectionId);
            State.Connections.Add(connection);
            _connectionsUpdated = true;
            return TaskDone.Done;
        }

        public Task Disconnect(DisconnectAction action)
        {
            action.EnsureNotNull().EnsureAuthorized(action.Meta.User.UserKey == this.GetPrimaryKey());

            var connection = new ConnectionData(action.Payload.ConnectionId);
            _connectionsUpdated = State.Connections.Remove(connection);

            return TaskDone.Done;
        }

        public Task<PlayerInfoPayload> GetInfo(GetPlayerInfoAction action)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerSettingsPayload> GetSettings(GetPlayerSettingsAction action)
        {
            action.EnsureNotNull();
            if (action.Meta.Auth.IsAuthenticated)
            {
                return Mapper.Map<PlayerSettingsPayload>(State.Settings);
            }
            using (var generator = new PlayerNameGenerator())
            {
                var name = await generator.Generate();
                var settings = new PlayerSettingsData {Name = name};
                State.PlayerKey = this.GetPrimaryKey();
                State.Settings = settings;
                await WriteStateAsync();
            }
            return Mapper.Map<PlayerSettingsPayload>(State.Settings);
        }

        public async Task<PlayerSettingsPayload> SaveSettings(SavePlayerSettingsAction action)
        {
            action.EnsureNotNull().EnsureAuthorized(action.Meta.User.UserKey == this.GetPrimaryKey());

            State.Settings = Mapper.Map<PlayerSettingsData>(action.Payload);
            await WriteStateAsync();
            return action.Payload;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            _stateUpdateTimer = RegisterTimer(SaveStateIfChanged, null, StateUpdateTimeout, StateUpdateInterval);
        }

        public override Task OnDeactivateAsync()
        {
            _stateUpdateTimer?.Dispose();
            return base.OnDeactivateAsync();
        }

        private async Task SaveStateIfChanged(object arg)
        {
            if (!_connectionsUpdated)
            {
                return;
            }
            await WriteStateAsync();
            _connectionsUpdated = false;
        }

        public class SystemAction<TState, TAction>
        {
            public TAction Action { get; set; }

            public TState State { get; }
        }

        public class SystemActionResults<TState>
        {
            public TState State { get; set; }

            public List<object> Events { get; set; }
        }

        public interface IGame
        {
        }
    }
}