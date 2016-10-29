using System;
using System.Threading.Tasks;
using AutoMapper;
using Conreign.Client.CommandHandler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.CommandHandler.Handlers
{
    internal class CancelFleetHandler : ICommandHandler<CancelFleetCommand, Unit>
    {
        private readonly IMapper _mapper;

        public CancelFleetHandler(IMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            _mapper = mapper;
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