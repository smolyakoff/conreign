using System;

namespace Conreign.Core.Utility
{
    internal static class ActionExtensions
    {
        public static T EnsureNotNull<T>(this T action) where T : class
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return action;
        }
    }
}