using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Api.Framework.Utility
{
    internal class ActionMapper
    {
        private readonly Dictionary<Type, ActionInfo> _actions;

        public ActionMapper()
        {
            _actions = new Dictionary<Type, ActionInfo>();
        }

        public object Map(GenericAction action, Type targetType)
        {
            var actionInfo = _actions.ContainsKey(targetType)
                ? _actions[targetType]
                : CollectInfo(targetType);
            _actions[targetType] = actionInfo;
            dynamic result = actionInfo.Constructor.Invoke(new object[] { });
            if (actionInfo.PayloadType != null)
            {
                result.Payload = action.Payload.ToObject(actionInfo.PayloadType);
            }
            if (actionInfo.MetaType != null)
            {
                result.Meta = action.Meta.ToObject(actionInfo.MetaType);
            }
            return result;
        }

        private static ActionInfo CollectInfo(Type actionType)
        {
            var constructor = actionType.GetConstructor(new Type[] { });
            if (constructor == null)
            {
                throw new InvalidOperationException($"Couldn't create action of type: {actionType.Name}. No default constructor.");
            }
            
            var payloadType = actionType.GetInterfaces()
                .Where(t => t.IsGenericTypeDefinition)
                .FirstOrDefault(t => t == typeof (IPayloadContainer<>))?.GenericTypeArguments.First();
            var metaType = actionType.GetInterfaces()
                .Where(t => t.IsGenericTypeDefinition)
                .FirstOrDefault(t => t == typeof(IMetadataContainer<>))?.GenericTypeArguments.First();
            return new ActionInfo
            {
                Constructor = constructor,
                PayloadType = payloadType,
                MetaType = metaType
            };
        }

        private class ActionInfo
        {
            public ConstructorInfo Constructor { get; set; }

            public Type PayloadType { get; set; }
            
            public Type MetaType { get; set; } 
        }
    }
}
