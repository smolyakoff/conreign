using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class ArriveAction : IGrainAction<IWorldGrain>
    {
        public GrainKey<IWorldGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IWorldGrain>(default(long));
    }
}
