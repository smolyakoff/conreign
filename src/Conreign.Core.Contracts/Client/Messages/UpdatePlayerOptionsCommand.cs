using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class UpdatePlayerOptionsCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public PlayerOptionsData Options { get; set; }
    }
}