using MediatR;

namespace Conreign.Framework.Contracts.Routing
{
    public class ChooseRouteRequest : IAsyncRequest<ChooseRouteResponse>
    {
        public string Key { get; set; }
    }
}
