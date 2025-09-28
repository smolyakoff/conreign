using Conreign.Server.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class UpdateGameOptionsHandler : IRequestHandler<UpdateGameOptionsCommand, Unit>
{
    private readonly HandlerContext _context;

    public UpdateGameOptionsHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateGameOptionsCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        await player.UpdateGameOptions(command.Options);
        return Unit.Value;
    }
}