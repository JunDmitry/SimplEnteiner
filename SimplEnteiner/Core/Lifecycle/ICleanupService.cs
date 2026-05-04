using System;

namespace SimplEnteiner.Core.Lifecycle
{
    internal interface ICleanupService : IDisposable, IAsyncDisposable
    {
        void AddIfDisposable(object instance, Action<object> onRelease = null);
    }
}
