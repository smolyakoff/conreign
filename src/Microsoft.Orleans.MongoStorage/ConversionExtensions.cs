using System;
using Orleans;
using Orleans.Runtime;

namespace Microsoft.Orleans.Storage
{
    internal static class ConversionExtensions
    {
        public static MongoGrain ToGrain(this IGrainState grainState, GrainReference reference, string grainType,
            Guid serviceId)
        {
            return new MongoGrain
            {
                Id = Conventions.PrimaryKeyForGrain(serviceId, reference),
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
                GrainStateType = state.GetType().FullName,
                GrainType = grainType,
                ServiceId = serviceId.ToString()
            };
        }
    }
}