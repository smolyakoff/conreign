using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core.Editor
{
    public class MapEditorState
    {
        public MapData Map { get; set; } = new MapData();
        public int NeutralPlanetsCount { get; set; } = Defaults.NeutralPlayersCount;
        public List<Guid> Players { get; set; } = new List<Guid>();
    }
}