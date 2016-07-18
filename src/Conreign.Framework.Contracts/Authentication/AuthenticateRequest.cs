using MediatR;

namespace Conreign.Framework.Contracts.Authentication
{
    public class AuthenticateRequest : IAsyncRequest<AuthenticateResponse>
    {
        public AuthenticateRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }        
    }
}