using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Client.Contracts.Messages;
using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class CancelFleetHandler : ICommandHandler<CancelFleetCommand, Unit>
    {
        private readonly IMapper _mapper;

        public CancelFleetHandler(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Unit> Handle(CommandEnvelope<CancelFleetCommand, Unit> message)
        {
            var command = message.Command;
            var context = message.Context;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            var cancelation = _mapper.Map<CancelFleetCommand, FleetCancelationData>(command);
            await player.CancelFleet(cancelation);
            return Unit.Value;
        }
    }
}