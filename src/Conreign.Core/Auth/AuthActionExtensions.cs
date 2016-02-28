using System;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth.Data;

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
            if (!action.Meta.Auth.IsAuthenticated)
            {
                throw new UserException(action.Meta.Auth.ErrorMessage);
            }
            return action;
        }
    }
}
