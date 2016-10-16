using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay.AI
{
    public interface IBotBattleStrategy
    {
        List<FleetData> ChooseFleetsToLaunch(Guid userId, ReadOnlyMap map);
    }
}