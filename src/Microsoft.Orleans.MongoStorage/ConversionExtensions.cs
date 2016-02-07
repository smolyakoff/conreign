using System;
using System.Collections.Generic;
using Orleans;
using Orleans.Runtime;

namespace Microsoft.Orleans.Storage
{
    internal static class ConversionExtensions
    {
        public static MongoGrain ToGrain(this GrainState state, GrainReference reference, string grainType,
            Guid serviceId)
        {
            return new MongoGrain
            {
                Id = Conventions.PrimaryKeyForGrain(serviceId, reference),
                Data = new Dictionary<string, object>(state.AsDictionary()),
                Meta = state.ToGrainMeta(reference, grainType, serviceId)
            };
        }

        public static MongoGrainMeta ToGrainMeta(this GrainState state, GrainReference reference, string grainType,
            Guid serviceId)
        {
            return new MongoGrainMeta
            {
                ETag = state.Etag,
                GrainId = reference.ToKeyString(),
                GrainStateType = state.GetType().FullName,
                GrainType = grainType,
                ServiceId = serviceId.ToString()
            };
        }
    }
}