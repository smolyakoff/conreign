using System;
using Conreign.Core.Contracts.Abstractions;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Immutable]
    public class DisconnectAction : IMetadataContainer<Meta>, IPayloadContainer<string>, IGrainAction<IWorldGrain>
    {
        public DisconnectAction(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
            Payload = connectionId;
        }

        public Meta Meta { get; set; }

        public string Payload { get; }

        public GrainKey<IWorldGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IWorldGrain>(default(long));
    }
}