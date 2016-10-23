using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class UpdatePlayerOptionsCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
        public string Nickname { get; set; }
        public string Color { get; set; }
    }
}
