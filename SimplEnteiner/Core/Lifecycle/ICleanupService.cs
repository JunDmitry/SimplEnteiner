using System;

namespace SimplEnteiner.Core.Lifecycle
{
    internal interface ICleanupService : IDisposable
    {
        void AddIfDisposable(object instance);
    }
}
