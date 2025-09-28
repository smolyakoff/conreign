namespace Conreign.Server.Core.Editor;

public class UniformRandomPlanetGeneratorOptions
{
    public UniformRandomPlanetGeneratorOptions(
        int minProductionRate,
        int maxProductionRate,
        int minShips,
        int maxShips,
        int minPower,
        int maxPower)
    {
        // TODO: argument checks
        MinProductionRate = minProductionRate;
        MaxProductionRate = maxProductionRate;
        MinShips = minShips;
        MaxShips = maxShips;
        MinPower = minPower;
        MaxPower = maxPower;
    }

    public static UniformRandomPlanetGeneratorOptions NeutralPlanetDefaults { get; }
        = new(
            3,
            10,
            3,
            5,
            20,
            85);

    public static UniformRandomPlanetGeneratorOptions PlayerPlanetDefaults { get; }
        = new(
            8,
            14,
            10,
            40,
            70,
            95);

    public int MinProductionRate { get; }
    public int MaxProductionRate { get; }
    public int MinShips { get; }
    public int MaxShips { get; }
    public int MinPower { get; }
    public int MaxPower { get; }
}