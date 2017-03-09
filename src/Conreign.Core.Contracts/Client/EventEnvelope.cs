using Conreign.Core.Contracts.Client.Serialization;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Client
{
    [Immutable]
    public class EventEnvelope
    {
        public EventEnvelope(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; }
        public string Type => Payload == null ? null : MessageTypeSerializer.Serialize(Payload.GetType());
    }
}