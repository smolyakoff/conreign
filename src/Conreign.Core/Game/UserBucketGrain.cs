using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Game.Data;
using Conreign.Core.Utility;
using Orleans;

namespace Conreign.Core.Game
{
    public class UserBucketGrain : Grain<UserBucketState>, IUserBucketGrain
    {
        private static IMapper Mapper { get; }

        static UserBucketGrain()
        {
            var configuration = new MapperConfiguration(cfg =>
                cfg.CreateMap<PlayerSettingsPayload, PlayerSettingsData>().ReverseMap());
            Mapper = configuration.CreateMapper();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            await ReadStateAsync();
            State.BucketKey = State.BucketKey ?? this.GetPrimaryKeyString();
        }

        public async Task<PlayerSettingsPayload> GetPlayerSettings(GetPlayerSettingsAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            PlayerData player = null;
            var key = ToStoredKey(action.Payload);
            if (State.Players.ContainsKey(key))
            {
                player = State.Players[key];
            }
            player = player ?? await CreatePlayer(key);
            return Mapper.Map<PlayerSettingsPayload>(player.Settings);
        }

        public async Task Connect(ConnectAction action)
        {
            action.EnsureNotNull();
            var key = ToStoredKey(action.Meta.User.UserKey);
            if (!State.Players.ContainsKey(key))
            {
                throw new InvalidOperationException("User not found in the bucket.");
            }
            State.Players[key].ConnectionIds.Add(action.Payload);
            await WriteStateAsync();
        }

        public async Task Disconnect(DisconnectAction action)
        {
            action.EnsureNotNull();
            var key = ToStoredKey(action.Meta.User.UserKey);
            if (!State.Players.ContainsKey(key))
            {
                return;
            }
            State.Players[key].ConnectionIds.Remove(action.Payload);
            await WriteStateAsync();
        }

        private async Task<PlayerData> CreatePlayer(string playerKey)
        {
            using (var generator = new PlayerNameGenerator())
            {
                var name = await generator.Generate();
                var player = new PlayerData
                {
                    PlayerKey = ToNormalKey(playerKey),
                    Settings = new PlayerSettingsData {Name = name}
                };
                State.Players[playerKey] = player;
                await WriteStateAsync();
                return player;
            }
        }

        private static string ToStoredKey(Guid playerKey)
        {
            return playerKey.ToString("N");
        }

        private static Guid ToNormalKey(string playerKey)
        {
            return Guid.ParseExact(playerKey, "N");
        }
    }
}
