using System;
using System.Collections.Generic;

namespace SimplEnteiner.Core.ScopeFeature
{
    internal class ResolutionContext : IDisposable
    {
        private bool _disposed;

        public ResolutionContext(Scope scope)
        {
            CurrentScope = scope;
        }

        public Scope CurrentScope { get; private set; }
        public Dictionary<Type, object> CachedInstances { get; } = new Dictionary<Type, object>();

        public void Dispose()
        {
            if (_disposed) 
                return;

            _disposed = true;
            CurrentScope = null;
            CachedInstances.Clear();
        }
    }
}
