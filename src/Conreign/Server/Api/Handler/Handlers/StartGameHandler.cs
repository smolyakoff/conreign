using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class StartGameHandler : IRequestHandler<StartGameCommand, Unit>
{
    private readonly HandlerContext _context;

    public StartGameHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(StartGameCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        await player.StartGame();
        return Unit.Value;
    }
}