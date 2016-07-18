using System;
using Conreign.Core.Contracts.Auth;
using Conreign.Framework.Contracts.Authentication;
using Conreign.Framework.Contracts.Core;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Core.Auth
{
    internal static class AuthActionExtensions
    {
        public static T EnsureAuthenticated<T>(this T action) where T : IMetadataContainer<IAuthMeta>
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (action.Meta.Auth.IsAuthenticated)
            {
                return action;
            }
            switch (action.Meta.Auth.Error)
            {
                case AuthenticationError.BadToken:
                    throw new UserException("Access token has invalid format.", AuthenticationError.BadToken.ToString());
                case AuthenticationError.TokenExpired:
                    throw new UserException("Access token has expired.", AuthenticationError.TokenExpired.ToString());
                case null:
                    throw new UserException("Authentication is required.", "AuthenticationRequired");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static T EnsureAuthorized<T>(this T action, bool condition) where T : IMetadataContainer<IAuthMeta>
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            action.EnsureAuthenticated();
            if (!condition)
            {
                throw new UserException("Action is not allowed.", AuthorizationError.Forbidden.ToString());
            }
            return action;
        }

        public static T EnsureAuthorized<T>(this T action, Predicate<T> predicate) where T : IMetadataContainer<IAuthMeta>
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            action.EnsureAuthenticated();
            if (!predicate(action))
            {
                throw new UserException("Action is not allowed.", AuthorizationError.Forbidden.ToString());
            }
            return action;
        }
    }
}