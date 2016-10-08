using System;

namespace Conreign.Core.Contracts.Presence
{
    [Serializable]
    public class UserStatusChanged
    {
        public bool IsOnline { get; set; }
        public Guid UserId { get; set; }
    }
}
