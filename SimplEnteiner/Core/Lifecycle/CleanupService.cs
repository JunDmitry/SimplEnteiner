using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplEnteiner.Core.Lifecycle
{
    internal class CleanupService : ICleanupService
    {
        private readonly List<IDisposable> _disposables;
        private readonly object _lock = new object();
        private bool _disposed;

        public CleanupService()
        {
            _disposables = new List<IDisposable>();
        }

        public void AddIfDisposable(object instance)
        {
            if (_disposed || instance == null)
                return;

            if (instance is IDisposable disposable)
            {
                lock(_lock)
                    _disposables.Add(disposable);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            List<IDisposable> disposables;

            lock (_lock)
            {
                disposables = _disposables.ToList();
                _disposables.Clear();
            }

            foreach (IDisposable disposable in disposables)
                disposable?.Dispose();
        }
    }
}
