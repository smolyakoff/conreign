using Conreign.Client.Contracts;

namespace Conreign.LoadTest.Core
{
    public interface IBotFactory
    {
        bool CanCreate { get; }
        Bot Create(IClientConnection connection);
    }
}