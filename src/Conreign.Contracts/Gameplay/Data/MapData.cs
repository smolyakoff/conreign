using System.Collections.Generic;

namespace Conreign.Contracts.Gameplay.Data
{
    public class MapData
    {
        public Dictionary<int, PlanetData> Planets { get; set; } = new Dictionary<int, PlanetData>();
        public int Width { get; set; } = Defaults.MapWidth;
        public int Height { get; set; } = Defaults.MapHeight;
    }
}