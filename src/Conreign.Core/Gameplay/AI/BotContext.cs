using System;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay.AI
{
    public class BotContext
    {
        public BotContext(Guid userId, IUser user)
        {
            UserId = userId;
            User = user;
        }

        public Guid UserId { get; }
        public IPlayer Player { get; set; }
        public IUser User { get; }
    }
}