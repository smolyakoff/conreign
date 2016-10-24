using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class EndTurnCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
    }
}
