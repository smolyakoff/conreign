using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class WriteHandler : ICommandHandler<WriteCommand, Unit>
    {
        private readonly IMapper _mapper;

        public WriteHandler(IMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CommandEnvelope<WriteCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            var textMessage = _mapper.Map<WriteCommand, TextMessageData>(command);
            await player.Write(textMessage);
            return Unit.Value;
        }
    }
}
