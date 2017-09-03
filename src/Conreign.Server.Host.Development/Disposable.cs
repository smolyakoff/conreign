using System;

namespace Conreign.Server.Host.Development
{
    internal class Disposable : IDisposable
    {
        private readonly Action _dispose;
        private bool _disposed;

        public Disposable(Action dispose)
        {
            if (dispose == null)
            {
                throw new ArgumentNullException(nameof(dispose));
            }
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _dispose();
            _disposed = true;
        }
    }
}