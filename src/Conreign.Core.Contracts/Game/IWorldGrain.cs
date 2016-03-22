using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using MediatR;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IWorldGrain : IGrainWithIntegerKey, IAsyncRequestHandler<ArriveAction, WelcomeMessagePayload>
    {
    }
}