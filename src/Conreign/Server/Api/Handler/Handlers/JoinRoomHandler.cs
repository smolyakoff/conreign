using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class JoinRoomHandler : IRequestHandler<JoinRoomCommand, Unit>
{
    private readonly HandlerContext _context;

    public JoinRoomHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(JoinRoomCommand command, CancellationToken cancellationToken)
    {
        await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        return Unit.Value;
    }
}