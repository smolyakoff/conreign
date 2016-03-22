using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Auth;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Core.Contracts.Game.Streaming;
using Conreign.Core.Game.Events;
using Conreign.Core.Utility;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;
using MediatR;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Game
{
    public class SubscribeToPlayerStreamAction: IGrainAction<GameRoomGrain>, IPayloadContainer<SubscribeToPlayerStreamPayload>, IAsyncRequest
    {
        public GrainKey<GameRoomGrain> GrainKey { get; }
    }

    public class SubscribeToPlayerStreamPayload
    {
        public Guid PlayerKey { get; }

        public string GameRoomKey { get; }
    }

    public class GameRoomGrain : Grain<GameRoomState>, IGameRoomGrain
    {
        private readonly GameRoom _gameRoom;

        public GameRoomGrain(IMediator mediator)
        {
       
            _gameRoom = new GameRoom(this.GetPrimaryKeyString(), mediator);
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var message = new StatefulAction<GameRoomState, InitializeGameRoomAction, Unit>(new InitializeGameRoomAction(), State);
            await _gameRoom.Handle(message);
        }

        public Task<GameRoomPayload> LookInto(LookIntoGameRoomAction action)
        {
            action.EnsureNotNull();
            return _gameRoom.Handle(new StatefulAction<GameRoomState, LookIntoGameRoomAction, GameRoomPayload>(action, State));
        }

        public async Task<GameRoomStatusPayload> Enter(EnterGameRoomAction action)
        {
            throw new NotImplementedException();
            action.EnsureNotNull().EnsureAuthorized(action.Payload.Key == this.GetPrimaryKeyString());
            var userKey = action.Meta.User.UserKey;

            room.Handle();

            if (State.Status == GameRoomStatus.Free)
            {
                var player = GrainFactory.GetGrain<IPlayerGrain>(userKey);
                var stream = GetStreamProvider(Streams.PlayerStreamProvider).GetPlayerStream(userKey);
                await stream.OnNextAsync(new PlayerConnectionsRequestedEvent(action.Meta));
                var info = await player.GetSettings(new GetPlayerInfoAction(userKey));
                var member = new GameRoomMemberData
                {
                    Name = settings.Name,
                    ConnectionIds = new HashSet<string>(),
                    StreamSubscription = await stream.SubscribeAsync(HandlePlayerEvent, e => TaskDone.Done)
                };
                State.Owner = member;
                State.Members.Add(member);
                State.Status = GameRoomStatus.Preparing;
                await WriteStateAsync();
                return new GameRoomStatusPayload {Status = State.Status};
            }
        }

        public Task Leave(LeaveGameRoomAction action)
        {
            throw new NotImplementedException();
        }

        private Task HandlePlayerEvent(IMetadataContainer<IUserMeta> @event, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }
    }


    public class GameRoomMemberData
    {
        public GameRoomMemberData()
        {
            ConnectionIds = new HashSet<string>();
        }

        public string Name { get; set; }

        public StreamSubscriptionHandle<IMetadataContainer<IUserMeta>> StreamSubscription { get; set; }

        public HashSet<string> ConnectionIds { get; set; }
    }

    public class GameRoomState : GrainState
    {
        public GameRoomState()
        {
            Status = GameRoomStatus.Free;
            Members = new List<GameRoomMemberData>();
        }

        public GameRoomStatus Status { get; set; }

        public GameRoomMemberData Owner { get; set; }

        public List<GameRoomMemberData> Members { get; set; }
    }
}