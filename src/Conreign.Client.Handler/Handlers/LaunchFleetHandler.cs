﻿using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class LaunchFleetHandler : ICommandHandler<LaunchFleetCommand, Unit>
    {
        public async Task<Unit> Handle(CommandEnvelope<LaunchFleetCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            await player.LaunchFleet(command.Fleet);
            return Unit.Value;
        }
    }
}