using System.Collections.Generic;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class MapData
    {
        public Dictionary<long, PlanetData> Planets { get; } = new Dictionary<long, PlanetData>();
        public int Width { get; set; } = Defaults.MapWidth;
        public int Height { get; set; } = Defaults.MapHeight;
    }
}