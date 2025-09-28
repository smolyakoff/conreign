using AutoMapper;
using Conreign.Server.Contracts.Client.Messages;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class CancelFleetHandler : IRequestHandler<CancelFleetCommand, Unit>
{
    private readonly HandlerContext _context;
    private readonly IMapper _mapper;

    public CancelFleetHandler(IMapper mapper, HandlerContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context;
    }

    public async Task<Unit> Handle(CancelFleetCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        var cancellation = _mapper.Map<CancelFleetCommand, FleetCancelationData>(command);
        await player.CancelFleet(cancellation);
        return Unit.Value;
    }
}