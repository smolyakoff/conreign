using System;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay.AI
{
    public interface IBotContext
    {
        IPlayer Player { get; set; }
        IUser User { get; }
        Guid UserId { get; }
    }
}