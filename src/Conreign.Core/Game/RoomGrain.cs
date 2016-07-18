using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Messages;
using MediatR;
using Orleans;

namespace Conreign.Core.Game
{
    public class RoomGrain : Grain<RoomState>, IRoomGrain
    {
        private readonly IMediator _mediator;
        private Room _room;

        public RoomGrain(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override Task OnActivateAsync()
        {
            State = new RoomState(this.GetPrimaryKeyString());
            _room = new Room(State, _mediator);
            return TaskDone.Done;
        }

        public Task<JoinRoomResponse> Handle(Message<JoinRoomRequest, JoinRoomResponse> message)
        {
            return _room.Handle(message);
        }

        public Task<GetRoomStateResponse> Handle(Message<GetRoomStateRequest, GetRoomStateResponse> message)
        {
            return _room.Handle(message);
        }
    }
}