using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI
{
    public class LogBehaviour : IBotBehaviour<IClientEvent>, IBotBehaviour<PlayerUpdated>
    {
        private readonly ConsoleColor _color;
        private string _name;

        public LogBehaviour(ConsoleColor color)
        {
            _color = color;
        }

        public Task Handle(IClientEvent @event, BotContext context)
        {
            Console.ForegroundColor = _color;
            Console.WriteLine($"[{_name ?? context.UserId.ToString()}]: {@event.GetType().Name}");
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;
        }

        public Task Handle(PlayerUpdated @event, BotContext context)
        {
            if (@event.Player.UserId == context.UserId)
            {
                _name = @event.Player.Nickname;
            }
            return Task.CompletedTask;
        }
    }
}
