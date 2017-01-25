using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class StartGameCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}