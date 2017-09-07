using System;
using System.Threading.Tasks;
using Conreign.Client.Contracts;
using Conreign.Contracts.Errors;
using MediatR;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;

namespace Conreign.Client.SignalR
{
    internal class SignalRSender
    {
        private const string OperationDescription = "SignalR.Send";
        private readonly IHubProxy _hub;
        private readonly HubConnection _hubConnection;
        private readonly ILogger _logger;
        private readonly Metadata _metadata;
        private readonly RetryPolicy _retryPolicy;

        public SignalRSender(IHubProxy hub, HubConnection hubConnection, Metadata metadata)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _logger = Log.Logger.ForContext(GetType());
            _retryPolicy = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry * 2));
        }

        public async Task<T> Send<T>(IRequest<T> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var meta = new Metadata
            {
                AccessToken = _metadata.AccessToken,
                TraceId = Guid.NewGuid().ToString()
            };
            var envelope = new MessageEnvelope
            {
                Meta = meta,
                Payload = command
            };
            var diagnosticProperties = new ILogEventEnricher[]
            {
                new PropertyEnricher("ConnectionId", _hubConnection.ConnectionId),
                new PropertyEnricher("TraceId", meta.TraceId),
                new PropertyEnricher("CommandType", command.GetType().Name)
            };
            using (LogContext.Push(diagnosticProperties))
            {
                var result = await _retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        using (_logger.BeginTimedOperation(OperationDescription))
                        {
                            return await SendInternal<T>(envelope);
                        }
                    }
                    catch (Exception ex)
                    {
                        var level = ex is UserException ? LogEventLevel.Warning : LogEventLevel.Error;
                        _logger.Write(level, ex, "[SignalRClient:{ConnectionId}:{TraceId}] {ErrorMessage}",
                            _hubConnection.ConnectionId,
                            envelope.Meta.TraceId,
                            ex.Message);
                        throw;
                    }
                });
                return result;
            }
        }

        private async Task<T> SendInternal<T>(MessageEnvelope envelope)
        {
            if (_hubConnection.State != ConnectionState.Connected)
            {
                _logger.Warning("SignalR hub connection is {ConnectionState}", _hubConnection.State);
            }
            if (_hubConnection.State == ConnectionState.Disconnected)
            {
                await _hubConnection.Start();
            }
            else if (_hubConnection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException($"Invalid connection state: {_hubConnection.State}.");
            }
            try
            {
                {
                    if (typeof(T) != typeof(Unit))
                    {
                        var response = await _hub.Invoke<MessageEnvelope>("Send", envelope).ConfigureAwait(false);
                        return (T) response.Payload;
                    }
                    await _hub.Invoke("Post", envelope);
                    return default(T);
                }
            }
            catch (HubException ex)
            {
                var jError = ex.ErrorData as JObject;
                if (jError == null)
                {
                    throw new ServerException(ex);
                }
                var typeToken = jError["$nettype"];
                if (typeToken == null)
                {
                    throw new ServerException(ex);
                }
                var type = Type.GetType(typeToken.ToString());
                var error = jError.ToObject(type) as UserError;
                if (error == null)
                {
                    throw new ServerException(ex);
                }
                throw error.ToUserException();
            }
        }
    }
}