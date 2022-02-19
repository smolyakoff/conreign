namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

public class PlanetData
{
    public string Name { get; set; }
    public int ProductionRate { get; set; }
    public double Power { get; set; }
    public int Ships { get; set; }
    public Guid? OwnerId { get; set; }
}