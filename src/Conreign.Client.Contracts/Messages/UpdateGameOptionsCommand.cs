using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class UpdateGameOptionsCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public GameOptionsData Options { get; set; }
    }
}