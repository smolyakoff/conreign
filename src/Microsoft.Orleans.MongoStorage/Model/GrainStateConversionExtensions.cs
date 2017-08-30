using System;
using Orleans;
using Orleans.Runtime;

namespace Microsoft.Orleans.MongoStorage.Model
{
    internal static class GrainStateConversionExtensions
    {
        public static MongoGrain ToGrain(this IGrainState grainState, GrainReference reference, string grainType,
            Guid serviceId)
        {
            return new MongoGrain
            {
                Id = Naming.PrimaryKeyForGrain(serviceId, reference),
                Data = grainState.State,
                Meta = grainState.ToGrainMeta(reference, grainType, serviceId)
            };
        }

        public static MongoGrainMeta ToGrainMeta(this IGrainState state, GrainReference reference, string grainType,
            Guid serviceId)
        {
            return new MongoGrainMeta
            {
                ETag = state.ETag,
                GrainId = reference.ToKeyString(),
                GrainStateType = state.State.GetType().FullName,
                GrainType = grainType,
                ServiceId = serviceId.ToString()
            };
        }
    }
}