namespace Conreign.Core.Contracts.Game.Messages
{
    public class PlanetState
    {
        public string Name { get; set; }

        public PositionState Position { get; set; }

        public PlayerState Owner { get; set; }
    }
}