using System.Threading.Tasks;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Utility;
using MediatR;

namespace Conreign.Core.Game

{
    public class InitializeGameRoomAction
    {
        
    }

    public class GameRoom : 
        IStatefulActionHandler<GameRoomState, LookIntoGameRoomAction, GameRoomPayload>,
        IStatefulActionHandler<GameRoomState, InitializeGameRoomAction, Unit>
    {
        private readonly string _key;
        private readonly IMediator _mediator;

        public GameRoom(string key, IMediator mediator)
        {
            _key = key;
            _mediator = mediator;
        }

        public async Task<GameRoomPayload> Handle(StatefulAction<GameRoomState, LookIntoGameRoomAction, GameRoomPayload> message)
        {
            var action = message.Action;
            var state = message.State;
            action.EnsureNotNull();
            var userKey = action.Meta.User.UserKey;

            if (state.Status == GameRoomStatus.Free)
            {
                var infoMessage = new GetPlayerInfoAction(userKey);
                var info = await _mediator.SendAsync(infoMessage);
                var member = new GameRoomMemberData
                {
                    Name = info.Settings.Name,
                    ConnectionIds = info.ConnectionIds
                };
                state.Owner = member;
                state.Members.Add(member);
                state.Status = GameRoomStatus.Preparing;
                return new GameRoomPayload {Name = "Abc", Status = state.Status};
            }

            return new GameRoomPayload
            {
            };

        }

        public async Task<Unit> Handle(StatefulAction<GameRoomState, InitializeGameRoomAction, Unit> message)
        {
            throw new System.NotImplementedException();
        }
    }
}