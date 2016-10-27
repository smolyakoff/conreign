using System;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Contracts.Client
{
    public class LoginResult
    {
        public LoginResult(IUser user, Guid userId, string accessToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            User = user;
            UserId = userId;
            AccessToken = accessToken;
        }

        public IUser User { get; }
        public Guid UserId { get; }
        public string AccessToken { get; }
    }
}