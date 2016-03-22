using System;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Framework.Contracts.Auth;
using Conreign.Framework.Contracts.Core.Data;
using MediatR;

namespace Conreign.Core.Auth.Actions
{
    public class LoginAnonymousAction : IPayloadContainer<KeyPayload<Guid>>, IAsyncRequest<AccessTokenPayload>
    {
        public LoginAnonymousAction(Guid playerKey)
        {
            Payload = new KeyPayload<Guid>(playerKey);
        }

        public KeyPayload<Guid> Payload { get; }
    }
}