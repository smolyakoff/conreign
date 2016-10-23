using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class UpdatePlayerOptionsHandler : IAsyncRequestHandler<UpdatePlayerOptionsCommand, Unit>
    {
        private readonly IHandlerContext _context;
        private readonly IMapper _mapper;

        public UpdatePlayerOptionsHandler(IHandlerContext context, IMapper mapper)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdatePlayerOptionsCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            var data = _mapper.Map<UpdatePlayerOptionsCommand, PlayerOptionsData>(message);
            await player.UpdateOptions(data);
            return Unit.Value;
        }
    }
}
