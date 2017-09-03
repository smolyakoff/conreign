using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.LoadTest.Core.Battle
{
    public interface IBotBattleStrategy
    {
        List<FleetData> ChooseFleetsToLaunch(Guid userId, ReadOnlyMap map);
    }
}