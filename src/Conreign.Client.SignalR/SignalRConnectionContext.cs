using Conreign.Core.Contracts.Client;
using Microsoft.AspNet.SignalR.Client;

namespace Conreign.Client.SignalR
{
    internal class SignalRConnectionContext
    {
        public SignalRConnectionContext(IHubProxy hub, HubConnection hubConnection, Metadata metadata)
        {
            Hub = hub;
            HubConnection = hubConnection;
            Metadata = metadata;
        }

        public IHubProxy Hub { get; }
        public HubConnection HubConnection { get; }
        public Metadata Metadata { get; }
    }
}