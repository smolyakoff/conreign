namespace Conreign.Server.Core.Utility;

public static class CancellationTokenExtensions
{
    public static Task AsTask(this CancellationToken token)
    {
        var tcs = new TaskCompletionSource<object>();
        token.Register(() => tcs.TrySetCanceled(), false);
        return tcs.Task;
    }
}