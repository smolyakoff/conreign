using System;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Core;
using Conreign.Framework.Core.Serialization;
using MediatR;
using Orleans;

namespace Conreign.Framework.Routing
{
    internal class OldRouter : IAsyncRequestHandler<Request, Response>
    {
        private readonly ActionMapper _actionMapper;
        private readonly IMediator _mediator;
        private readonly RouteTable _routingTable;

        public OldRouter(RouteTable routeTable, ActionMapper actionMapper, IMediator mediator)
        {
            if (routeTable == null)
            {
                throw new ArgumentNullException(nameof(routeTable));
            }
            if (actionMapper == null)
            {
                throw new ArgumentNullException(nameof(actionMapper));
            }
            _routingTable = routeTable;
            _actionMapper = actionMapper;
            _mediator = mediator;
        }

        public async Task<Response> Handle(Request request)
        {
            if (!GrainClient.IsInitialized)
            {
                return Response.Problem(ErrorFactory.ServiceUnavailable());
            }
            var route = _routingTable.Match(request.Type);
            if (route == null)
            {
                return Response.Problem(ErrorFactory.HandlerNotFound(request?.Type ?? "<No Type>"));
            }
            dynamic action = _actionMapper.Map(request, route.RequestType);
            var key = action.GrainKey.Key;
            var grain = GrainFactoryExtensions.GetGrain(GrainClient.GrainFactory, route.Method.DeclaringType, key);
            var result = await route.Method.Invoke(grain, new object[] {action});
            return Response.Success(result);
        }
    }
}