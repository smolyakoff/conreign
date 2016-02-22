using System;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Auth.Data
{
    public class AuthenticationStatusPayload
    {
        public bool IsAuthenticated { get; set; }

        public Guid UserKey { get; set; }

        public UserMessage ErrorMessage { get; set; }
    }
}