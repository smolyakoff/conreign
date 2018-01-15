using System;

namespace Conreign.Contracts.Gameplay.Data
{
    public class GameOptionsData : IEquatable<GameOptionsData>
    {
        public const int DefaultNeutralPlayersCount = 10;
        public const int DefaultBotsCount = 0;
        public const int DefaultMapWidth = 8;
        public const int DefaultMapHeight = 8;
        public const int MinumumMapSize = 4;
        public const int MaximumMapSize = 20;

        public int MapWidth { get; set; } = DefaultMapWidth;
        public int MapHeight { get; set; } = DefaultMapHeight;
        public int NeutralPlanetsCount { get; set; } = DefaultNeutralPlayersCount;
        public int BotsCount { get; set; } = DefaultBotsCount;

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
                   NeutralPlanetsCount == other.NeutralPlanetsCount && BotsCount == other.BotsCount;
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
            return obj.GetType() == GetType() && Equals((GameOptionsData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MapWidth;
                hashCode = (hashCode * 397) ^ MapHeight;
                hashCode = (hashCode * 397) ^ NeutralPlanetsCount;
                hashCode = (hashCode * 397) ^ BotsCount;
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