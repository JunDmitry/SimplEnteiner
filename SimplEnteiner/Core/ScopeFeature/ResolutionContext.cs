using System;
using System.Collections.Concurrent;

namespace SimplEnteiner.Core.ScopeFeature
{
    internal class ResolutionContext : IDisposable
    {
        private bool _disposed;

        public ResolutionContext(Scope scope, object id = null, Type requestType = null)
        {
            CurrentScope = scope;
            Id = id;
            RequestType = requestType;
        }

        public Scope CurrentScope { get; private set; }
        public object Id { get; }
        public Type RequestType { get; set; }
        public ConcurrentDictionary<Type, object> CachedInstances { get; } = new ConcurrentDictionary<Type, object>();

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
