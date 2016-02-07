using System.Threading.Tasks;
using Orleans;

namespace Conreign.Core.Contracts
{
    public interface IGameGrain : IGrainWithStringKey
    {
        Task<string> SayWelcomeAsync(string user);

        Task ClearAsync();
    }
}
