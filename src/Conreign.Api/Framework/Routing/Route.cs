using System;
using System.Reflection;

namespace Conreign.Api.Framework.Routing
{
    public class Route
    {
        internal Route(Type actionType, MethodInfo method)
        {
            if (actionType == null)
            {
                throw new ArgumentNullException(nameof(actionType));
            }
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            ActionType = actionType;
            Method = method;
        }

        public Type ActionType { get; private set; }

        public MethodInfo Method { get; private set; }
    }
}
