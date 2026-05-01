using System;
using System.Collections.Generic;

namespace SimplEnteiner.Core.ScopeFeature
{
    internal class ResolutionContext : IDisposable
    {
        private bool _disposed;

        public ResolutionContext(Scope scope, object id = null)
        {
            CurrentScope = scope;
            Id = id;
        }

        public Scope CurrentScope { get; private set; }
        public object Id { get; }
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
