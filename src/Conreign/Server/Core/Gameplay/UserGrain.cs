using Conreign.Server.Contracts.Server.Gameplay;
using Conreign.Server.Contracts.Server.Presence;
using Conreign.Server.Contracts.Shared.Gameplay;
using Conreign.Server.Core.Communication;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Server.Core.Gameplay;

[StatelessWorker]
public class UserGrain : Grain<Guid>, IUserGrain
{
    public async Task<IPlayer> JoinRoom(string roomId, string connectionId)
    {
        var userId = this.GetPrimaryKey();
        var connection = GrainFactory.GetGrain<IConnectionGrain>(connectionId);
        var topicId = TopicIds.Player(userId, roomId);
        var player = GrainFactory.GetGrain<IPlayerGrain>(userId, roomId);
        await player.Ping();
        await connection.Connect(topicId);
        return player;
    }
}