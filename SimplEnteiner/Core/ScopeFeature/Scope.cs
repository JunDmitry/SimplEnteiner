using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;
using SimplEnteiner.Core.Lifecycle;
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

        private readonly List<Scope> _childrens;
        private readonly object _childrensLock = new object();

        private readonly ICleanupService _cleanupService;
        private readonly IInterfaceInvoker _interfaceInvoker;

        private bool _disposed;

        public Scope(ResolverFunc resolver)
        {
            _resolver = resolver.ThrowIfArgumentNull();
            Parent = null;
            _singletons = new Dictionary<Type, object>();
            _scopedInstances = new Dictionary<Type, object>();
            _childrens = new List<Scope>();
            _cleanupService = new CleanupService();
            _interfaceInvoker = new InterfaceInvoker();
        }

        protected Scope(Scope parent)
        {
            Parent = parent.ThrowIfArgumentNull();
            _resolver = Parent._resolver;
            _singletons = parent._singletons;
            _scopedInstances = new Dictionary<Type, object>();
            _childrens = new List<Scope>();
            _cleanupService = new CleanupService();
            _interfaceInvoker = new InterfaceInvoker();
        }

        public Scope Parent { get; }
        public bool IsRoot => Parent == null;

        public void Dispose()
        {
            if (_disposed) 
                return;

            _disposed = true;

            lock(_scopedLock)
            {
                _scopedInstances.Clear();
            }

            if (IsRoot)
            {
                lock (_singletons)
                {
                    foreach (var instance in _singletons.Values)
                        _cleanupService.AddIfDisposable(instance);

                    _singletons.Clear();
                }
            }
            else
            {
                lock (Parent._childrensLock)
                    Parent._childrens.Remove(this);
            }
            
            _cleanupService.Dispose();
        }

        public virtual object Resolve(Type interfaceType)
        {
            return _resolver(interfaceType, this);
        }

        public virtual T Resolve<T>()
        {
            return (T) _resolver(typeof(T), this);
        }

        public virtual async Task<object> ResolveAsync(Type interfaceType)
        {
            object instance = Resolve(interfaceType);

            await _interfaceInvoker.InvokeAsync<ILateInitializable>(instance);

            return instance;
        }

        public virtual async Task<T> ResolveAsync<T>()
        {
            return (T) await ResolveAsync(typeof(T));
        }

        public virtual object Resolve(Type interfaceType, object id)
        {
            return _resolver(interfaceType, this, id);
        }

        public virtual T Resolve<T>(object id)
        {
            return (T)_resolver(typeof(T), this, id);
        }

        public virtual async Task<object> ResolveAsync(Type interfaceType, object id)
        {
            object instance = Resolve(interfaceType, id);

            await _interfaceInvoker.InvokeAsync<ILateInitializable>(instance);

            return instance;
        }

        public virtual async Task<T> ResolveAsync<T>(object id)
        {
            return (T)await ResolveAsync(typeof(T), id);
        }

        public virtual IScope CreateScope()
        {
            Scope child = new Scope(this);

            lock (_childrens)
                _childrens.Add(child);

            return child;
        }

        public virtual void Install(IInstaller installer)
        {
            installer.ThrowIfArgumentNull().Install(this);
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

        public IBindingDecorate<TService> Decorate<TService>()
        {
            BindingBuilderInternal builderInternal = new BindingBuilderInternal(typeof(TService));

            return new BindingDecorate<TService>(builderInternal, this);
        }

        public IBindingDecorate Decorate(Type interfaceType)
        {
            interfaceType.ThrowIfArgumentNull();
            BindingBuilderInternal builderInternal = new BindingBuilderInternal(interfaceType);

            return new BindingDecorate(builderInternal, this);
        }

        void IBindingTarget.Register(BindingBuilderInternal bindingBuilder)
        {
            if (bindingBuilder.IsRegistered)
                return;

            bindingBuilder.ExecuteAllStages();
            _registry.Add(bindingBuilder);
            bindingBuilder.MarkRegistered();
        }

        void IBindingTarget.RegisterDecorator(BindingBuilderInternal bindingBuilder)
        {
            Type interfaceType = bindingBuilder.InterfaceType;
            Type decoratorType = bindingBuilder.ImplementationType;

            ConstructorInfo ctor = decoratorType.GetInjectableConstructor(Constants.InjectAttributeType)
                ?? throw new ArgumentException($"No constructor for decorator {decoratorType}");
            Func<object[], object> factory = ctor.GetFactoryMethod();
            DecoratorRegistration registration = new DecoratorRegistration(interfaceType, decoratorType, bindingBuilder.Order, bindingBuilder.LifeTime, ctor, factory);

            _registry.AddDecorator(registration);
        }

        internal void ValidateAll()
        {
            _registry.ValidateAll();
        }

        internal void Start()
        {
            Registration registration;

            foreach (KeyValuePair<Type, Registration> pair in _registry.ExactBindings)
            {
                registration = pair.Value;

                if (registration.Instance != null)
                {
                    _interfaceInvoker.Invoke<IStartable>(registration.Instance);
                }
                else if (registration.Lifetime == LifeTime.Singleton)
                {
                    object instance = Resolve(pair.Key);
                    _interfaceInvoker.Invoke<IStartable>(instance);
                }
            }

            //foreach (List<DecoratorRegistration> decorators in _registry.DecoratorBindings.Values)
            //{
            //    foreach (DecoratorRegistration decorator in decorators)
            //    {

            //    }
            //}

            for (int i = 0; i < _childrens.Count; i++)
                _childrens[i].Start();
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

        internal Registration FindConditionalRegistration(Type interfaceType, object id)
        {
            ConditionalKey key = new ConditionalKey(interfaceType, id);

            for (Scope scope = this; scope != null; scope = scope.Parent)
            {
                if (scope._registry.ConditionalBindings.TryGetValue(key, out Registration registration))
                    return registration;
            }

            return null;
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

        internal List<DecoratorRegistration> GetDecoratorRegistrations(Type interfaceType)
        {
            List<DecoratorRegistration > registrations = new List<DecoratorRegistration>();
            Stack<Scope> scopes = new Stack<Scope>();
            
            for (Scope scope = this; scope != null; scope = scope.Parent)
                scopes.Push(scope);

            while (scopes.Count > 0)
            {
                Scope scope = scopes.Pop();

                if (scope._registry.DecoratorBindings.TryGetValue(interfaceType, out List<DecoratorRegistration> inner))
                    registrations.AddRange(inner);
            }

            return registrations;
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
                TrackDisposable(instance);
            }
        }

        internal void TrackDisposable(object instance)
        {
            _cleanupService.AddIfDisposable(instance);
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
