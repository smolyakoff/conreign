using System;

namespace Conreign.Server.Auth
{
    public class AuthOptions
    {
        public AuthOptions()
        {
            TokenLifetimeInSeconds = (int) TimeSpan.FromDays(3).TotalSeconds;
        }

        public string JwtSecret { get; set; }

        public int TokenLifetimeInSeconds { get; set; }
    }
}