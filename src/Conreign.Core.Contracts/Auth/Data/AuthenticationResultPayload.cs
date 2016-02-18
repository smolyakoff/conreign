using System;

namespace Conreign.Core.Contracts.Auth.Data
{
    public class AuthenticationResultPayload
    {
        public Guid PlayerKey { get; set; }

        public string Token { get; set; }
    }
}