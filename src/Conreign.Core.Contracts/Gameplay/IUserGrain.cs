using System.Threading.Tasks;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IUserGrain : IGrainWithGuidKey, IUser, IPlayerFactory
    {
    }

    public interface ICollectorGrain : IGrainWithIntegerKey
    {
        Task Collect(object @event);
    }
}