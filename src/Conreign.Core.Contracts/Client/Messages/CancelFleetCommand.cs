using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class CancelFleetCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
        public int Index { get; set; }
    }
}