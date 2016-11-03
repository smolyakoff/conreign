using System;
using System.Threading.Tasks;
using Conreign.Core.AI.Behaviours.Events;

namespace Conreign.Core.AI.Behaviours
{
    public class JoinRoomBehaviour : IBotBehaviour<BotAuthenticated>
    {
        private readonly TimeSpan _delay;
        private readonly string _roomId;

        public JoinRoomBehaviour(string roomId, TimeSpan delay)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            _roomId = roomId;
            _delay = delay;
        }

        public JoinRoomBehaviour(string roomId) : this(roomId, TimeSpan.FromSeconds(1))
        {
        }

        public async Task Handle(IBotNotification<BotAuthenticated> notification)
        {
            var context = notification.Context;
            await Task.Delay(_delay);
            context.Player = await context.User.JoinRoom(_roomId, context.Connection.Id);
            context.Logger = context.Logger.ForContext("RoomId", _roomId);
        }
    }
}