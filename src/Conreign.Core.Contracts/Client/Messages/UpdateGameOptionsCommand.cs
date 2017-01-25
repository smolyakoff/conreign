using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class UpdateGameOptionsCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public GameOptionsData Options { get; set; }
    }
}