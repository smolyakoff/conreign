using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class LaunchFleetHandler : IRequestHandler<LaunchFleetCommand, Unit>
{
    private readonly HandlerContext _context;

    public LaunchFleetHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(LaunchFleetCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        await player.LaunchFleet(command.Fleet);
        return Unit.Value;
    }
}