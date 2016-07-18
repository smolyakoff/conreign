using MediatR;

namespace Conreign.Core.Contracts.Game.Messages
{
    public class JoinRoomRequest : IAsyncRequest<JoinRoomResponse>
    {
        public string RoomId { get; set; }
        public string Nickname { get; set; }
    }
}