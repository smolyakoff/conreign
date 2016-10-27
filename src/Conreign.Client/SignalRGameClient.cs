using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Client;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Conreign.Client
{
    public class SerilogTextWriter : TextWriter
    {
        private readonly ILogger _logger;

        public SerilogTextWriter(ILogger logger)
        {
            _logger = logger;
        }

        public override Encoding Encoding { get; }


        public override void WriteLine(string value)
        {
            _logger.Verbose(value);
        }
    }

    public class SignalRGameClient : IGameClient
    {
        private readonly SignalRGameClientOptions _options;
        private readonly ConcurrentDictionary<Guid, Task<IClientConnection>> _connections;

        public SignalRGameClient(SignalRGameClientOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
            _connections = new ConcurrentDictionary<Guid, Task<IClientConnection>>();
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
                var connection = new SignalRClientConnection(hubConnection, gameHub);
                await hubConnection.Start(new ServerSentEventsTransport());
                return connection;
            });
        }
    }
}
