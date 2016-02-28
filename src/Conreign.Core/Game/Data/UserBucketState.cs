using System.Collections.Generic;
using Orleans;

namespace Conreign.Core.Game.Data
{
    public class UserBucketState : GrainState
    {
        public UserBucketState()
        {
            Players = new Dictionary<string, PlayerData>();
        }

        public string BucketKey { get; set; }

        public Dictionary<string, PlayerData> Players { get; set; }
    }
}