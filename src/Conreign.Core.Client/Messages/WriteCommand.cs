using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class WriteCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
        public string Text { get; set; }
    }
}