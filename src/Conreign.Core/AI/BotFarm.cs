using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Conreign.Core.AI.Events;
using Conreign.Core.Contracts.Client;
using Serilog;

namespace Conreign.Core.AI
{
    public class BotFarm : IDisposable
    {
        private readonly string _id;
        private readonly IClient _client;
        private readonly IBotFactory _botFactory;
        private List<Bot> _bots;
        private bool _isDisposed;
        private IDisposable _logSubscription;
        private readonly ILogger _logger;

        public BotFarm(string id, IClient client, IBotFactory botFactory)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (botFactory == null)
            {
                throw new ArgumentNullException(nameof(botFactory));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            _id = id;
            _client = client;
            _botFactory = botFactory;
            _bots = new List<Bot>();
            Completion = Task.FromResult(0);
            _logger = Log.Logger.ForContext("BotFarmId", id);
        }

        public async Task Start()
        {
            EnsureIsNotDisposed();
            _logger.Information("[{BotFarmId}] Farm is starting...");
            while (_botFactory.CanCreate)
            {
                var connectionId = Guid.NewGuid();
                var connection = await _client.Connect(connectionId);
                var bot = _botFactory.Create(connection);
                _bots.Add(bot);
                _logger.Information("[{BotFarmId}] Bot {BotId} is connected. Connection id is {ConnectionId}.",
                    _id,
                    bot.Id,
                    connectionId);
            }
            Completion = Task.WhenAll(_bots.Select(x => x.Completion));
            _logSubscription = _bots.Select(x => x.Events).Merge().Subscribe(OnNext, OnError, OnCompleted);
            foreach (var bot in _bots)
            {
                bot.Start();
            }
            _logger.Information("[{BotFarmId}] Farm is started.");
        }

        public void Stop()
        {
            EnsureIsNotDisposed();
            _logger.Information("[{BotFarmId}] Farm stop requested...");
            foreach (var bot in _bots)
            {
                bot.Stop();
            }
        }

        public Task Completion { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (!disposing)
            {
                return;
            }
            foreach (var bot in _bots)
            {
                bot.Dispose();
                bot.Connection.Dispose();
            }
            _logSubscription.Dispose();
            _logSubscription = null;
            _bots = null;
            _isDisposed = true;
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("BotFarm");
            }
        }

        private void OnCompleted()
        {
            _logger.Information("[{BotFarmId}] Event stream completed.", _id);
        }

        private void OnError(Exception ex)
        {
            _logger.Error(ex, "[{BotFarmId}] Error: {Message}", _id, ex.Message);
        }

        private void OnNext(IBotEvent @event)
        {
            _logger.Debug("[{BotFarmId}]. Event: {@Event}.", _id, @event);
        }
    }
}
