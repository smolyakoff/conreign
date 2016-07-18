using System;
using System.Security.Claims;
using Conreign.Framework.Contracts.Authentication;

namespace Conreign.Core.Contracts.Communication
{
    public class Metadata : IAuthenticationMetadata
    {
        public string ConnectionId { get; set; }

        public string AccessToken { get; set; }

        public ClaimsPrincipal Principal { get; set; }

        public AuthenticationError? AuthenticationError { get; set; }

        public Guid? UserId
        {
            get
            {
                var claim = Principal?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return null;
                }
                return Guid.Parse(claim.Value);
            }
        }
    }
}