using AutoMapper;
using Conreign.Server.Contracts.Client.Messages;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Api.Handler.Handlers;

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, Unit>
{
    private readonly HandlerContext _context;
    private readonly IMapper _mapper;

    public SendMessageHandler(IMapper mapper, HandlerContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context;
    }

    public async Task<Unit> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var player = await _context.User.JoinRoom(command.RoomId, _context.Connection.Id);
        var textMessage = _mapper.Map<SendMessageCommand, TextMessageData>(command);
        await player.SendMessage(textMessage);
        return Unit.Value;
    }
}