namespace Conreign.Core.Client
{
    public class Message<T>
    {
        public MessageMetadata Meta { get; set; }
        public T Payload { get; set; }
    }
}