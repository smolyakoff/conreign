using System;
using System.Collections.Generic;

namespace Conreign.Framework.Routing
{
    public interface IRouteKeyConvention
    {
        string GetKey(Type requestType, Type responseType, IEnumerable<Type> otherResponseTypes);
    }
}