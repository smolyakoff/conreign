using Conreign.Core.Contracts.Client.Serialization;
using Newtonsoft.Json;

namespace Conreign.Core.Contracts.Client
{
    [JsonConverter(typeof(MessageEnvelopeJsonConverter))]
    public class MessageEnvelope
    {
        public MessageEnvelope()
        {
            Meta = new Metadata();
        }

        public object Payload { get; set; }
        public Metadata Meta { get; set; }
        public string Type => Payload == null ? null : MessageTypeSerializer.Serialize(Payload.GetType());
    }
}