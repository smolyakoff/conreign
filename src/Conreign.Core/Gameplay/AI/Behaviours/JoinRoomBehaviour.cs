using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay.AI.Behaviours.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class JoinRoomBehaviour : IBotBehaviour<BotAuthenticated>
    {
        private readonly TimeSpan _delay;
        private readonly string _name;
        private readonly string _roomId;

        public JoinRoomBehaviour(string roomId, string name, TimeSpan delay)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            _roomId = roomId;
            _name = name;
            _delay = delay;
        }

        public async Task Handle(IBotNotification<BotAuthenticated> notification)
        {
            var context = notification.Context;
            await Task.Delay(_delay);
            context.Player = await context.User.JoinRoom(_roomId, context.Connection.Id);
            context.Logger = context.Logger.ForContext("RoomId", _roomId);
            await context.Player.UpdateOptions(new PlayerOptionsData
            {
                Nickname = _name,
                Color = "#000000"
            });
        }
    }
}