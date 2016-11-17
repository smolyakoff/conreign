using System.Threading;
using System.Threading.Tasks;

namespace Conreign.Core.Utility
{
    public static class CancellationTokenExtensions
    {
        public static Task AsTask(this CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            return tcs.Task;
        }
    }
}
