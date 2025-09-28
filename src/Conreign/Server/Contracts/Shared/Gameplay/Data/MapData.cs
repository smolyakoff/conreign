namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

public class MapData
{
    public Dictionary<int, PlanetData> Planets { get; set; } = new();
    public int Width { get; set; } = Defaults.MapWidth;
    public int Height { get; set; } = Defaults.MapHeight;
}