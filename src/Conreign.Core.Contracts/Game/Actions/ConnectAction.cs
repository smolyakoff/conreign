using System;
using Conreign.Core.Contracts.Abstractions;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Immutable]
    public class ConnectAction : IMetadataContainer<Meta>, IPayloadContainer<string>, IGrainAction<IWorldGrain>
    {
        public ConnectAction(string connectionId)
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