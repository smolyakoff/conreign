using System;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth.Data;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class ArriveAction : IGrainAction<IWorldGrain>, IMetadataContainer<IUserMeta>
    {
        public GrainKey<IWorldGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IWorldGrain>(default(long));

        public IUserMeta Meta { get; set; }
    }
}
