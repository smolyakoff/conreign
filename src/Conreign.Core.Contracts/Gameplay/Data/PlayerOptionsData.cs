using System;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class PlayerOptionsData : IEquatable<PlayerOptionsData>
    {
        public string Nickname { get; set; }
        public string Color { get; set; }

        public bool Equals(PlayerOptionsData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Nickname, other.Nickname) && string.Equals(Color, other.Color);
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
            return Equals((PlayerOptionsData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Nickname?.GetHashCode() ?? 0)*397) ^ (Color?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(PlayerOptionsData left, PlayerOptionsData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PlayerOptionsData left, PlayerOptionsData right)
        {
            return !Equals(left, right);
        }
    }
}