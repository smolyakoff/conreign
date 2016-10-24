using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class CancelFleetHandler : IAsyncRequestHandler<CancelFleetCommand, Unit>
    {
        private readonly IHandlerContext _context;
        private readonly IMapper _mapper;

        public CancelFleetHandler(IHandlerContext context, IMapper mapper)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CancelFleetCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            var cancelation = _mapper.Map<CancelFleetCommand, FleetCancelationData>(message);
            await player.CancelFleet(cancelation);
            return Unit.Value;
        }
    }
}