using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class StartGameCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
    }
}
