using System;
using System.Collections.Generic;
using System.Linq;
using SimplEnteiner.Core.ResolverService;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ScopeFeature
{
    public class Scope : IScope
    {
        private readonly ResolverFunc _resolver;
        private readonly Scope _parent;

        private readonly Dictionary<Type, object> _singletons;
        private readonly object _singletonLock = new object();

        private readonly Dictionary<Type, object> _scopedInstances;
        private readonly object _scopedLock = new object();

        private bool _disposed;

        public Scope(ResolverFunc resolver)
        {
            _resolver = resolver.ThrowIfArgumentNull();
            _parent = null;
            _singletons = new Dictionary<Type, object>();
            _scopedInstances = new Dictionary<Type, object>();
        }

        protected Scope(Scope parent)
        {
            _parent = parent.ThrowIfArgumentNull();
            _resolver = _parent._resolver;
            _singletons = parent._singletons;
            _scopedInstances = new Dictionary<Type, object>();
        }

        public bool IsRoot => _parent == null;

        public void Dispose()
        {
            if (_disposed) 
                return;

            _disposed = true;
            List<object> instances;

            lock(_scopedLock)
            {
                instances = _scopedInstances.Values.ToList();
                _scopedInstances.Clear();
            }

            foreach (object instance in instances)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public virtual object Resolve(Type interfaceType)
        {
            return _resolver(interfaceType, this);
        }

        public virtual T Resolve<T>()
        {
            return (T) _resolver(typeof(T), this);
        }

        public virtual IScope CreateScope()
        {
            return new Scope(this);
        }

        internal object GetSingleton(Type interfaceType)
        {
            lock (_singletonLock)
            {
                _singletons.TryGetValue(interfaceType, out object instance);
                return instance;
            }
        }

        internal object GetScoped(Type interfaceType)
        {
            lock (_scopedLock)
            {
                _scopedInstances.TryGetValue(interfaceType, out object instance);
                return instance;
            }
        }

        internal void StoreSingleton(Type interfaceType, object instance)
        {
            lock (_singletonLock)
            {
                _singletons[interfaceType] = instance;
            }
        }

        internal void StoreScoped(Type interfaceType, object instance)
        {
            lock (_scopedLock)
            {
                _scopedInstances[interfaceType] = instance;
            }
        }
    }
}
