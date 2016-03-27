using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using MediatR;

namespace Conreign.Core.Contracts.Game
{
    public interface IWorld : IAsyncRequestHandler<ArriveAction, WelcomeMessagePayload>
    {
    }
}
