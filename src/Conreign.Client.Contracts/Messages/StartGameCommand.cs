using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class StartGameCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}