using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using Conreign.Api.Framework.Routing;
using Conreign.Api.Framework.Utility;
using MediatR;
using Orleans;

namespace Conreign.Api.Framework
{
    public class ActionHandler : IAsyncRequestHandler<GenericAction, GenericActionResult>
    {
        private readonly RoutingTable _routingTable;

        private readonly ActionMapper _actionMapper;

        public ActionHandler(RoutingTable routeTable)
        {
            if (routeTable == null)
            {
                throw new ArgumentNullException(nameof(routeTable));
            }
            _routingTable = routeTable;
            _actionMapper = new ActionMapper();
        }

        public async Task<GenericActionResult> Handle(GenericAction message)
        {
            if (!GrainClient.IsInitialized)
            {
                var error = SystemError.ServiceUnavailable();
                return GenericActionResult.FromError(error);
            }
            var route = _routingTable.Match(message);
            if (route == null)
            {
                return GenericActionResult.FromError(UserError.NotFound());
            }
            dynamic action = _actionMapper.Map(message, route.ActionType);
            var key = action.GrainKey.Key;
            var grain = GrainFactoryExtensions.GetGrain(GrainClient.GrainFactory, route.Method.DeclaringType, key);
            var result = await route.Method.Invoke(grain, new object[]{action});
            var code = result == null ? HttpStatusCode.NoContent : HttpStatusCode.OK;
            return new GenericActionResult(code, result);
        }
    }
}
