using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class ArriveAction : IGrainAction<IWorldGrain>, IMetadataContainer<Meta>
    {
        public GrainKey<IWorldGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IWorldGrain>(default(long));

        public Meta Meta { get; set; }
    }
}
