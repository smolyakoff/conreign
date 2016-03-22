using Conreign.Framework.Contracts.Core.Data;
using MediatR;

namespace Conreign.Framework.Contracts.Auth
{
    public class AuthenticateAction : IPayloadContainer<AccessTokenPayload>, IAsyncRequest<AuthenticationResultPayload>
    {
        public AuthenticateAction(AccessTokenPayload payload)
        {
            Payload = payload;
        }

        public AccessTokenPayload Payload { get; }
    }
}