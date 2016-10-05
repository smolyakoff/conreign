namespace Conreign.Core.Contracts.Presence
{
    public class ConnectCommand
    {
        public string ConnectionId { get; set; }
        public IDisconnectable Connection { get; set; }
    }
}