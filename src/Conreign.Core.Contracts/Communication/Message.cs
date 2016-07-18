using Conreign.Framework.Contracts.Communication;
using MediatR;

namespace Conreign.Core.Contracts.Communication
{
    public static class Message
    {
        public static Message<TIn, TOut> Create<TIn, TOut>(TIn payload, Metadata metadata)
        {
            return new Message<TIn, TOut>(payload, metadata);
        }

        public static Message<TIn, Unit> Create<TIn>(TIn payload, Metadata metadata)
        {
            return new Message<TIn, Unit>(payload, metadata);
        }
    }

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