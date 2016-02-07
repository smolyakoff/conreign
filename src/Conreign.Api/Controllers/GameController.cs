using System;
using System.Threading.Tasks;
using System.Web.Http;
using Conreign.Core.Contracts;
using Orleans;

namespace Conreign.Api.Controllers
{
    [RoutePrefix("game")]
    public class GameController : ApiController
    {
        [HttpGet]
        [Route("welcome")]
        public Task<string> Welcome(string user)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IGameGrain>("game");
            return grain.SayWelcomeAsync(user);
        }

        [HttpGet]
        [Route("clear")]
        public async Task<string> Clear()
        {
            var grain = GrainClient.GrainFactory.GetGrain<IGameGrain>("game");
            await grain.ClearAsync();
            return "cleared";
        }
    }
}
