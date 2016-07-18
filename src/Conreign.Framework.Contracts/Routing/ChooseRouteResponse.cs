using System;

namespace Conreign.Framework.Contracts.Routing
{
    public class ChooseRouteResponse
    {
        public Type PayloadType { get; set; }

        public Type MetaType { get; set; }
    }
}