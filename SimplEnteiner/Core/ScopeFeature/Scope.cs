using System;
using System.Collections.Generic;
using System.Linq;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.RegistrationService;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ScopeFeature
{
    public class Scope : IScope, IBindingTarget
    {
        private readonly ResolverFunc _resolver;
        private readonly Registry _registry;

        private readonly Dictionary<Type, object> _singletons;
        private readonly object _singletonLock = new object();

        private readonly Dictionary<Type, object> _scopedInstances;
        private readonly object _scopedLock = new object();

        private bool _disposed;

        public Scope(ResolverFunc resolver)
        {
            _resolver = resolver.ThrowIfArgumentNull();
            Parent = null;
            _singletons = new Dictionary<Type, object>();
            _scopedInstances = new Dictionary<Type, object>();
        }

        protected Scope(Scope parent)
        {
            Parent = parent.ThrowIfArgumentNull();
            _resolver = Parent._resolver;
            _singletons = parent._singletons;
            _scopedInstances = new Dictionary<Type, object>();
        }

        public Scope Parent { get; }
        public bool IsRoot => Parent == null;

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

        public IBindingTo<T> Bind<T>()
        {
            BindingBuilderInternal builderInternal = new BindingBuilderInternal(typeof(T));

            return new BindingTo<T>(builderInternal, this);
        }

        public IBindingTo Bind(Type interfaceType)
        {
            interfaceType.ThrowIfArgumentNull();

            BindingBuilderInternal builderInternal = new BindingBuilderInternal(interfaceType);

            return new BindingTo(builderInternal, this);
        }

        void IBindingTarget.Register(BindingBuilderInternal bindingBuilder)
        {
            if (bindingBuilder.IsRegistered)
                return;

            bindingBuilder.ExecuteAllStages();
            _registry.Add(bindingBuilder);
            bindingBuilder.MarkRegistered();
        }

        internal void ValidateAll()
        {
            _registry.ValidateAll();
        }

        internal void AddRegister(BindingBuilderInternal builder)
        {
            _registry.Add(builder);
        }

        internal IReadOnlyDictionary<Type, Registration> GetAllExactRegistration()
        {
            return GetAllRegistration(s => s._registry.ExactBindings);
        }
        internal IReadOnlyDictionary<Type, Registration> GetAllOpenGenericRegistration()
        {
            return GetAllRegistration(s => s._registry.OpenGenericBindings);
        }

        internal Registration FindExactRegistration(Type interfaceType)
        {
            return FindRegistration(interfaceType, s => s._registry.ExactBindings);
        }

        internal Registration FindOpenGenericRegistration(Type interfaceType)
        {
            return FindRegistration(interfaceType, s => s._registry.OpenGenericBindings);
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

        private IReadOnlyDictionary<Type, Registration> GetAllRegistration(Func<Scope, IReadOnlyDictionary<Type, Registration>> selector)
        {
            Dictionary<Type, Registration> allRegistrations = new Dictionary<Type, Registration>();
            Stack<Scope> allScopes = new Stack<Scope>();

            for (Scope scope = this; scope != null; scope = scope.Parent)
                allScopes.Push(scope);

            while (allScopes.Count > 0)
            {
                IReadOnlyDictionary<Type, Registration> registrations = selector(allScopes.Pop())
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach (Type type in registrations.Keys)
                    allRegistrations[type] = registrations[type];
            }

            return allRegistrations;
        }

        private Registration FindRegistration(Type interfaceType, Func<Scope, IReadOnlyDictionary<Type, Registration>> selector)
        {
            for (Scope scope = this; scope != null; scope = scope.Parent)
            {
                if (selector(scope).TryGetValue(interfaceType, out Registration registration))
                    return registration;
            }

            return null;
        }
    }
}
