using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Client.Contracts;
using Conreign.LoadTest.Core.Events;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace Conreign.LoadTest.Core
{
    public class BotFarm
    {
        private const string OperationDescription = "BotFarm.Run";
        private readonly IBotFactory _botFactory;
        private readonly IClient _client;
        private readonly string _id;
        private readonly ILogger _logger;
        private readonly BotFarmOptions _options;

        public BotFarm(string id, IClient client, IBotFactory botFactory, BotFarmOptions options)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (botFactory == null)
            {
                throw new ArgumentNullException(nameof(botFactory));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            _id = id;
            _client = client;
            _botFactory = botFactory;
            _options = options;
            _logger = Log.Logger.ForContext("BotFarmId", id);
        }

        public async Task Run(CancellationToken? cancellationToken = null)
        {
            IDisposable logSubscription = null;
            var bots = new List<Bot>();
            try
            {
                cancellationToken = cancellationToken ?? CancellationToken.None;
                _logger.Information("[BotFarm:{BotFarmId}] Farm is starting...");
                while (_botFactory.CanCreate)
                {
                    var connectionId = Guid.NewGuid();
                    var connection = await _client.Connect(connectionId);
                    var bot = _botFactory.Create(connection);
                    bots.Add(bot);
                    _logger.Information(
                        "[BotFarm:{BotFarmId}] Bot {BotId} is connected. Connection id is {ConnectionId}.",
                        _id,
                        bot.Id,
                        connectionId);
                }
                logSubscription = bots
                    .Select(x => x.Events.Catch<IBotEvent, Exception>(e =>
                    {
                        OnError(e);
                        return Observable.Empty<IBotEvent>();
                    }))
                    .Merge()
                    .Subscribe(OnNext, OnError, OnCompleted);
                var tasks = new List<Task>();
                foreach (var bot in bots)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        _logger.Information("[BotFarm:{BotFarmId}] Started {BotId}.", _id, bot.Id);
                        try
                        {
                            await bot.Run(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var level = ex is TaskCanceledException ? LogEventLevel.Warning : LogEventLevel.Error;
                            _logger.Write(
                                level,
                                level == LogEventLevel.Warning ? null : ex,
                                "[BotFarm:{BotFarmId}] Bot stopped with an error: {ErrorMessage}.",
                                _id,
                                ex.Message);
                            throw;
                        }
                        finally
                        {
                            _logger.Information("[BotFarm:{BotFarmId}] Stopped {BotId}.", _id, bot.Id);
                        }
                    }));
                    await Task.Delay(_options.BotStartInterval, cancellationToken.Value);
                }
                _logger.Information("[BotFarm:{BotFarmId}] Farm is started.");
                using (LogContext.PushProperty("BotFarmId", _id))
                using (_logger.BeginTimedOperation(OperationDescription))
                {
                    var tcs = new TaskCompletionSource<object>();
                    cancellationToken.Value.Register(async () =>
                    {
                        await Task.Delay(_options.GracefulStopPeriod);
                        tcs.SetCanceled();
                    });
                    await Task.WhenAny(Task.WhenAll(tasks), tcs.Task);
                }
            }
            finally
            {
                foreach (var bot in bots)
                {
                    bot.Dispose();
                    bot.Connection.Dispose();
                }
                logSubscription?.Dispose();
            }
        }

        private void OnCompleted()
        {
            _logger.Information("[BotFarm:{BotFarmId}] Event stream completed.", _id);
        }

        private void OnNext(IBotEvent @event)
        {
            _logger.Debug("[BotFarm:{BotFarmId}]. Event: {@Event}.", _id, @event);
        }

        private void OnError(Exception ex)
        {
            _logger.Error(ex, "[BotFarm:{BotFarmId}] Error: {Message}", _id, ex.Message);
        }
    }
}