using System.Threading.Tasks;
using Conreign.Framework.Contracts.Routing;
using MediatR;

namespace Conreign.Framework.Routing
{
    public class Router : IAsyncRequestHandler<ChooseRouteRequest, ChooseRouteResponse>
    {
        public Task<ChooseRouteResponse> Handle(ChooseRouteRequest message)
        {
        }
    }
}
