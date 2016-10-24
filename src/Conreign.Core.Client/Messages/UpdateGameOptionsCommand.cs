using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class UpdateGameOptionsCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
        public GameOptionsData Options { get; set; }
    }
}
