using System;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class GameOptionsData : IEquatable<GameOptionsData>
    {
        public GameOptionsData()
        {
            MapHeight = 8;
            MapWidth = 8;
            NeutralPlanets = 10;
        }

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int NeutralPlanets { get; set; }

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
            return MapWidth == other.MapWidth && MapHeight == other.MapHeight && NeutralPlanets == other.NeutralPlanets;
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
                hashCode = (hashCode*397) ^ MapHeight;
                hashCode = (hashCode*397) ^ NeutralPlanets;
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