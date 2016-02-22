using System;
using Conreign.Core.Contracts.Auth.Data;

namespace Conreign.Core.Contracts.Abstractions
{
    [Serializable]
    public class Meta : IAuthMeta, IUserMeta
    {
        public AuthPayload Auth { get; set; }

        public AuthenticationStatusPayload User { get; set; }
    }
}
