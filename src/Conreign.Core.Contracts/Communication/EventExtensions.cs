using System;
using System.Collections.Generic;
using System.Reflection;

namespace Conreign.Core.Contracts.Communication
{
    public static class EventExtensions
    {
        private static readonly Dictionary<Type, bool> PersistentFlags = new Dictionary<Type, bool>();

        public static bool IsPersistent(this IClientEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var type = @event.GetType();
            if (!PersistentFlags.ContainsKey(type))
            {
                PersistentFlags[type] = type.GetCustomAttribute(typeof(PersistentAttribute)) != null;
            }
            return PersistentFlags[type];
        }
    }
}
