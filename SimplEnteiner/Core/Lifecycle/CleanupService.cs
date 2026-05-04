using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplEnteiner.Core.Lifecycle
{
    internal class CleanupService : ICleanupService
    {
        private readonly List<(IDisposable Disposable, Action<object> OnRelease)> _disposables;
        private readonly object _lock = new object();
        private bool _disposed;

        public CleanupService()
        {
            _disposables = new List<(IDisposable Disposable, Action<object> OnRelease)>();
        }

        public void AddIfDisposable(object instance, Action<object> onRelease = null)
        {
            if (_disposed || instance == null)
                return;

            if (instance is IDisposable disposable)
            {
                lock(_lock)
                    _disposables.Add((disposable, onRelease));
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            List<(IDisposable Disposable, Action<object> OnRelease)> disposables;

            lock (_lock)
            {
                disposables = _disposables.ToList();
                _disposables.Clear();
            }

            foreach ((IDisposable Disposable, Action<object> OnRelease) in disposables)
            {
                OnRelease?.Invoke(Disposable);
                Disposable?.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;
            List<(IDisposable Disposable, Action<object> onRelease)> disposables;

            lock (_lock)
            {
                disposables = _disposables.ToList();
                _disposables.Clear();
            }

            foreach ((IDisposable Disposable, Action<object> onRelease) in disposables)
            {
                onRelease?.Invoke(Disposable);

                if (Disposable is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    Disposable?.Dispose();
            }
        }
    }
}
