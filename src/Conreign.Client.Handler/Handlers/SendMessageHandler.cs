using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class SendMessageHandler : ICommandHandler<SendMessageCommand, Unit>
    {
        private readonly IMapper _mapper;

        public SendMessageHandler(IMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            _mapper = mapper;
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