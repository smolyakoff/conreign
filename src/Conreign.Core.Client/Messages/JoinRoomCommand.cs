using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class JoinRoomCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
    }
}