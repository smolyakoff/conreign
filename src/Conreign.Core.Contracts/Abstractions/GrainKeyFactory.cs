using System;
using Orleans;

namespace Conreign.Core.Contracts.Abstractions
{
    public static class GrainKeyFactory
    {
        public static GrainKey<TGrain> KeyOrNullFor<TGrain>(string key) where TGrain : IGrainWithStringKey
        {
            return string.IsNullOrEmpty(key) ? null : new GrainKey<TGrain>(key);
        }

        public static GrainKey<TGrain> KeyOrNullFor<TGrain>(Guid? key) where TGrain : IGrainWithGuidKey
        {
            return key.HasValue ? new GrainKey<TGrain>(key.Value) : null;
        }

        public static GrainKey<TGrain> KeyOrNullFor<TGrain>(long? key) where TGrain : IGrainWithIntegerKey
        {
            return key.HasValue ? new GrainKey<TGrain>(key.Value) : null;
        }
    }
}