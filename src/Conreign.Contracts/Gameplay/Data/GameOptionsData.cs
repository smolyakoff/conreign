using System;

namespace Conreign.Contracts.Gameplay.Data
{
    public class GameOptionsData : IEquatable<GameOptionsData>
    {
        public GameOptionsData()
        {
            MapHeight = Defaults.MapHeight;
            MapWidth = Defaults.MapWidth;
            NeutralPlanetsCount = Defaults.NeutralPlayersCount;
        }

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int NeutralPlanetsCount { get; set; }

        public bool Equals(GameOptionsData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return MapWidth == other.MapWidth && MapHeight == other.MapHeight &&
                   NeutralPlanetsCount == other.NeutralPlanetsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((GameOptionsData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MapWidth;
                hashCode = (hashCode * 397) ^ MapHeight;
                hashCode = (hashCode * 397) ^ NeutralPlanetsCount;
                return hashCode;
            }
        }

        public static bool operator ==(GameOptionsData left, GameOptionsData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GameOptionsData left, GameOptionsData right)
        {
            return !Equals(left, right);
        }
    }
}