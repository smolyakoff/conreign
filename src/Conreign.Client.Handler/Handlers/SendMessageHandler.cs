using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Client.Contracts.Messages;
using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class SendMessageHandler : ICommandHandler<SendMessageCommand, Unit>
    {
        private readonly IMapper _mapper;

        public SendMessageHandler(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Unit> Handle(CommandEnvelope<SendMessageCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            var textMessage = _mapper.Map<SendMessageCommand, TextMessageData>(command);
            await player.SendMessage(textMessage);
            return Unit.Value;
        }
    }
}