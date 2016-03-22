using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;

namespace Conreign.Framework.Core.Serialization
{
    internal class ActionMapper
    {
        private readonly IConverter _converter;
        private readonly Dictionary<Type, ActionInfo> _actions;

        public ActionMapper(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            _converter = converter;
            _actions = new Dictionary<Type, ActionInfo>();
        }

        public object Map(Request action, Type targetType)
        {
            var actionInfo = _actions.ContainsKey(targetType)
                ? _actions[targetType]
                : CollectInfo(targetType);
            _actions[targetType] = actionInfo;
            dynamic result = actionInfo.Constructor.Invoke(new object[] {});
            if (actionInfo.PayloadType != null)
            {
                dynamic payload = _converter.Convert(action.Payload, actionInfo.PayloadType);
                result.Payload = payload;
            }
            if (actionInfo.MetaType != null)
            {
                dynamic meta = _converter.Convert(action.Meta, actionInfo.MetaType); 
                result.Meta = meta;
            }
            return result;
        }

        private static ActionInfo CollectInfo(Type actionType)
        {
            var constructor = actionType.GetConstructor(new Type[] {});
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"Couldn't create action of type: {actionType.Name}. No default constructor.");
            }

            var payloadType = actionType.GetInterfaces()
                .Where(t => t.IsConstructedGenericType)
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IPayloadContainer<>))?
                .GetGenericArguments()
                .First();
            var metaType = actionType.GetInterfaces()
                .Where(t => t.IsConstructedGenericType)
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IMetadataContainer<>))?
                .GetGenericArguments()
                .First();
            if (metaType?.IsInterface == true)
            {
                metaType = null;
            }
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