using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class WriteHandler : IAsyncRequestHandler<WriteCommand, Unit>
    {
        private readonly IHandlerContext _context;
        private readonly IMapper _mapper;

        public WriteHandler(IHandlerContext context, IMapper mapper)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            _context = context;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(WriteCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            var textMessage = _mapper.Map<WriteCommand, TextMessageData>(message);
            await player.Write(textMessage);
            return Unit.Value;
        }
    }
}
