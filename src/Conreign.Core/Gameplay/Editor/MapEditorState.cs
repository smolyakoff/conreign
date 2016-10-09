using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay.Editor
{
    public class MapEditorState
    {
        public MapData Map { get; } = new MapData();
        public int NeutralPlanetsCount { get; set; } = Defaults.NeutralPlayersCount;
        public List<Guid> Players { get; set; } = new List<Guid>();
    }
}