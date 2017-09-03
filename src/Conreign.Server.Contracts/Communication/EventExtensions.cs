using System;
using System.Collections.Generic;
using System.Reflection;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Contracts.Communication
{
    public static class EventExtensions
    {
        private static readonly Dictionary<(Type type, Type attribute), bool> AttributeFlags =
            new Dictionary<ValueTuple<Type, Type>, bool>();

        public static bool IsPersistent(this IClientEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            return @event.HasAttribute<PersistentAttribute>();
        }

        public static bool IsPrivate(this IClientEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            return @event.HasAttribute<PrivateAttribute>();
        }

        public static bool IsPublic(this IClientEvent @event)
        {
            return !@event.IsPrivate();
        }

        private static bool HasAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            var type = obj.GetType();
            var attributeType = typeof(TAttribute);
            var key = (type, attributeType);
            if (!AttributeFlags.ContainsKey(key))
            {
                AttributeFlags[key] = type.GetCustomAttribute(attributeType) != null;
            }
            return AttributeFlags[key];
        }
    }
}