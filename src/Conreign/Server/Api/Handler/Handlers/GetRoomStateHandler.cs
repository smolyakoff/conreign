using Conreign.Server.Contracts.Client.Messages;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class GetRoomStateHandler : IRequestHandler<GetRoomStateCommand, IRoomData>
{
    private readonly HandlerContext _context;

    public GetRoomStateHandler(HandlerContext context)
    {
        _context = context;
    }

    public async Task<IRoomData> Handle(GetRoomStateCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        return await player.GetState();
    }
}