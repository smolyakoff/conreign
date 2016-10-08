namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class MapData
    {
        public PlanetData[,] Cells { get; set; } = new PlanetData[0, 0];
    }
}