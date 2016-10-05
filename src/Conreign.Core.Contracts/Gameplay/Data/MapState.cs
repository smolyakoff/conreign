namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class MapState
    {
        public PlanetState[,] Cells { get; set; } = new PlanetState[0, 0];
    }
}