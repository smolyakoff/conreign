using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class UpdatePlayerOptionsHandler : IRequestHandler<UpdatePlayerOptionsCommand, Unit>
{
    private readonly HandlerContext _context;

    public UpdatePlayerOptionsHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdatePlayerOptionsCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        await player.UpdateOptions(command.Options);
        return Unit.Value;
    }
}