using Conreign.Client.Contracts.Serialization;
using Newtonsoft.Json;

namespace Conreign.Client.Contracts
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