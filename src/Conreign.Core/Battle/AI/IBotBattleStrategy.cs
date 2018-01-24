using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core.Battle.AI
{
    public interface IBotBattleStrategy
    {
        List<FleetData> ChooseFleetsToLaunch(Guid userId, IReadOnlyMap map);
    }
}