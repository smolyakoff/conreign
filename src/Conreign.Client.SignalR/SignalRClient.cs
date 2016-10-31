using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Exceptions;
using Microsoft.AspNet.SignalR.Client;
using Polly;
using Polly.Retry;
using Serilog;

namespace Conreign.Client.SignalR
{
    public class SignalRClient : IClient
    {
        private readonly ConcurrentDictionary<Guid, Task<IClientConnection>> _connections;
        private readonly SignalRClientOptions _options;
        private readonly RetryPolicy _connectionPolicy;

        public SignalRClient(SignalRClientOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
            _connections = new ConcurrentDictionary<Guid, Task<IClientConnection>>();
            _connectionPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(attempt*2));
        }

        public Task<IClientConnection> Connect(Guid connectionId)
        {
            return _connections.GetOrAdd(connectionId, async id =>
            {
                var hubConnection = new HubConnection(_options.ConnectionUri)
                {
                    TraceLevel = TraceLevels.All,
                    TraceWriter = new SerilogTextWriter(Log.Logger),
                    ConnectionId = connectionId.ToString(),
                };
                var gameHub = hubConnection.CreateHubProxy("GameHub");
                try
                {
                    if (_options.IsDebug)
                    {
                        await hubConnection.Start();
                    }
                    else
                    {
                        var result = await _connectionPolicy.ExecuteAndCaptureAsync(hubConnection.Start);
                        if (result.Outcome == OutcomeType.Failure)
                        {
                            throw result.FinalException;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ConnectionException($"Failed to connect to the API: {ex.Message}", ex);
                }
                var connection = new SignalRClientConnection(hubConnection, gameHub);
                return connection;
            });
        }
    }
}