using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class JoinRoomCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
    }
}