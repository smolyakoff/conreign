using Conreign.Core.Contracts.Client;

namespace Conreign.Core.AI
{
    public interface IBotFactory
    {
        Bot Create(IClientConnection connection);
        bool CanCreate { get; }
    }
}