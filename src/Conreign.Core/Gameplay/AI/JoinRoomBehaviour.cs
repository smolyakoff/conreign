using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay.AI
{
    public class JoinRoomBehaviour : ILifetimeBotBehaviour
    {
        private readonly string _roomId;
        private readonly string _name;
        private readonly TimeSpan _delay;

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

        public async Task Start(BotContext context)
        {
            await Task.Delay(_delay);
            context.Player = await context.User.JoinRoom(_roomId);
            await context.Player.UpdateOptions(new PlayerOptionsData
            {
                Nickname = _name,
                Color = "#000000"
            });
        }
    }
}
