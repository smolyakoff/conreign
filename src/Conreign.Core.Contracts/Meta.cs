using System;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Data;
using Conreign.Core.Contracts.Game;

namespace Conreign.Core.Contracts
{
    [Serializable]
    public class Meta : IAuthMeta, IUserMeta
    {
        public AuthMeta Auth { get; set; }

        public UserMeta User { get; set; }
    }
}
