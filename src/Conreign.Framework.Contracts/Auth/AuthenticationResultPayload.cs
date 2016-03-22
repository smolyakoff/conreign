using System;

namespace Conreign.Framework.Contracts.Auth
{
    public class AuthenticationResultPayload
    {
        public object User { get; set; }

        public object Auth { get; set; }
    }
}