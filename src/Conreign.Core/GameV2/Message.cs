namespace Conreign.Core.GameV2
{
    public class Message<TIn, TOut> : IMessage<TIn, Metadata, TOut> 
    {
        public Message(TIn payload, Metadata metadata)
        {
            Payload = payload;
            Meta = metadata;
        }

        public TIn Payload { get; }

        public Metadata Meta { get; }
    }
}