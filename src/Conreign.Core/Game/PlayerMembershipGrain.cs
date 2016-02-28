using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Utility;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Game
{
    [StatelessWorker]
    public class PlayerMembershipGrain: Grain, IPlayerMembershipGrain
    {
        public Task<PlayerSettingsPayload> GetPlayerSettings(GetPlayerSettingsAction action)
        {
            action.EnsureNotNull();
            var bucket = GetUserBucket(action.Payload);
            return bucket.GetPlayerSettings(action);
        }

        public Task Connect(ConnectAction action)
        {
            action.EnsureNotNull();
            var bucket = GetUserBucket(action.Meta.User.UserKey);
            return bucket.Connect(action);
        }

        public Task Disconnect(DisconnectAction action)
        {
            action.EnsureNotNull();
            var bucket = GetUserBucket(action.Meta.User.UserKey);
            return bucket.Disconnect(action);
        }

        private IUserBucketGrain GetUserBucket(Guid playerKey)
        {
            var bucketKey = DetermineBucketKey(playerKey);
            var bucket = GrainFactory.GetGrain<IUserBucketGrain>(bucketKey);
            return bucket;
        }

        private static string DetermineBucketKey(Guid playerKey)
        {
            return new string(playerKey.ToString("N").Take(3).OrderBy(x => x).ToArray());
        }
    }
}
