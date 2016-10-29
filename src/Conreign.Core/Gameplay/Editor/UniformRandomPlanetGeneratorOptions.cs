namespace Conreign.Core.Gameplay.Editor
{
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
            = new UniformRandomPlanetGeneratorOptions(
                minProductionRate: 3,
                maxProductionRate: 10,
                minShips: 3,
                maxShips: 5,
                minPower: 20,
                maxPower: 85);

        public static UniformRandomPlanetGeneratorOptions PlayerPlanetDefaults { get; }
            = new UniformRandomPlanetGeneratorOptions(
                minProductionRate: 8,
                maxProductionRate: 14,
                minShips: 10,
                maxShips: 40,
                minPower: 70,
                maxPower: 95);

        public int MinProductionRate { get; }
        public int MaxProductionRate { get; }
        public int MinShips { get; }
        public int MaxShips { get; }
        public int MinPower { get; }
        public int MaxPower { get; }
    }
}