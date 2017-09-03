using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class UpdatePlayerOptionsCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public PlayerOptionsData Options { get; set; }
    }
}