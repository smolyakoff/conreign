using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class EndTurnCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
    }
}