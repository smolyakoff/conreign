using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
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
        private CancellationTokenSource _disposalCancellationTokenSource;

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
            _logger = Log.Logger.ForContext("BotFarmId", id);
        }

        public async Task Run(CancellationToken? cancellationToken = null)
        {
            EnsureIsNotDisposed();
            cancellationToken = cancellationToken ?? CancellationToken.None;
            _disposalCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken.Value,
                _disposalCancellationTokenSource.Token).Token;
            _logger.Information("[{BotFarmId}] Farm is starting...");
            Task[] tasks;
            using (_logger.BeginTimedOperation("Creating bot factory"))
            {
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
                _logSubscription = _bots
                    .Select(x => x.Events.Catch<IBotEvent, Exception>(e =>
                    {
                        OnError(e);
                        return Observable.Empty<IBotEvent>();
                    }))
                    .Merge()
                    .Subscribe(OnNext, OnError, OnCompleted);
                tasks = _bots.Select(x => x.Run(token)).ToArray();
            }
            _logger.Information("[{BotFarmId}] Farm is started.");
            using (_logger.BeginTimedOperation("Running bot factory"))
            {
                await Task.WhenAll(tasks);
            }
        }

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
            _disposalCancellationTokenSource.Cancel();
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

        private void OnNext(IBotEvent @event)
        {
            _logger.Debug("[{BotFarmId}]. Event: {@Event}.", _id, @event);
        }

        private void OnError(Exception ex)
        {
            _logger.Error(ex, "[{BotFarmId}] Error: {Message}", _id, ex.Message);
        }
    }
}
