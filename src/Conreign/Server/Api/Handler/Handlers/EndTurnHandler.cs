using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class EndTurnHandler : IRequestHandler<EndTurnCommand, Unit>
{
    private readonly HandlerContext _context;

    public EndTurnHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(EndTurnCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        await player.EndTurn();
        return Unit.Value;
    }
}