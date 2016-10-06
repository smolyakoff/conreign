using System;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Commands;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Gameplay
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private Player _player;
        private IAsyncStream<MessageEnvelope> _communicationStream;

        public override Task OnActivateAsync()
        {
            _player = new Player(
                State, 
                GrainFactory.GetGrain<IUniverseGrain>(default(long)),
                this);
            var provider = GetStreamProvider(StreamConstants.DefaultProviderName);
            _communicationStream = provider.GetStream<MessageEnvelope>(StreamConstants.ClientStreamKey, StreamConstants.ClientStreamNamespace);
            return Task.CompletedTask;
        }

        public Task JoinRoom(JoinRoomCommand command)
        {
            return _player.JoinRoom(command);
        }

        public async Task UpdateOptions(UpdatePlayerOptionsCommand command)
        {
            await _player.UpdateOptions(command);
            await WriteStateAsync();
        }

        public Task UpdateGameOptions()
        {
            throw new NotImplementedException();
        }

        public async Task StartGame(StartGameCommand command)
        {
            await _player.StartGame(command);
            await WriteStateAsync();
        }

        public Task LaunchFleet()
        {
            throw new NotImplementedException();
        }

        public Task EndTurn()
        {
            throw new NotImplementedException();
        }

        public Task Write(WriteCommand command)
        {
            return _player.Write(command);
        }

        public Task<IRoomState> GetState()
        {
            throw new NotImplementedException();
        }

        public Task Disconnect(DisconnectCommand command)
        {
            return _player.Disconnect(command);
        }

        public Task Notify(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var envelope = new MessageEnvelope(@event, State.ConnectionIds.ToArray());
            return _communicationStream.OnNextAsync(envelope);
        }
    }
}