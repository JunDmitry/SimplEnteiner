using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.Configuration;
using SimplEnteiner.Core.ConventionBinding.Implementations;
using SimplEnteiner.Core.ConventionBinding.Interfaces;
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

        private readonly Scope _root;
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

        internal Scope(ResolverFunc resolver, ScopeConfig scopeConfig) : this(resolver)
        {
            InitializeFromDto(scopeConfig);
        }

        internal Scope(Scope parent, ScopeConfig scopeConfig) : this(parent)
        {
            InitializeFromDto(scopeConfig);
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

            _root = FindRoot();
        }

        private Scope FindRoot()
        {
            Scope scope = this;

            while (scope.Parent != null)
                scope = scope.Parent;

            return scope;
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

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            lock (_scopedLock)
            {
                _scopedInstances.Clear();
            }

            if (IsRoot)
            {
                lock (_singletons)
                {
                    _singletons.Clear();
                }
            }
            else
            {
                lock (Parent._childrensLock)
                    Parent._childrens.Remove(this);
            }

            await _cleanupService.DisposeAsync();
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

            await _interfaceInvoker.InvokeAsync<IAsyncInitializable>(instance);

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

            await _interfaceInvoker.InvokeAsync<IAsyncInitializable>(instance);

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

        public void BindConvention(Action<IConventionBuilder> configure)
        {
            ConventionBuilder builder = new ConventionBuilder(this);
            configure.ThrowIfArgumentNull().Invoke(builder);

            builder.Build();
        }

        public void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute)
        {
            _registry.AnalyzeReachability(roots, injectAttribute);

            foreach (Scope children in _childrens)
                children.AnalyzeReachability(roots, injectAttribute);
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
            List<DecoratorRegistration> registrations = new List<DecoratorRegistration>();
            List<Scope> scopes = new List<Scope>();

            for (Scope scope = this; scope != null; scope = scope.Parent)
                scopes.Add(scope);

            AddExactDecoratorRegistrations(interfaceType, registrations, scopes);

            if (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false))
                AddGenericDecoratorRegistrations(interfaceType, registrations, scopes);

            scopes.Clear();

            return registrations;
        }

        internal void StoreSingleton(Type interfaceType, object instance, Action<object> onRelease = null)
        {
            lock (_singletonLock)
            {
                _singletons[interfaceType] = instance;
                _root._cleanupService.AddIfDisposable(instance, onRelease);
            }
        }

        internal void StoreScoped(Type interfaceType, object instance, Action<object> onRelease = null)
        {
            lock (_scopedLock)
            {
                _scopedInstances[interfaceType] = instance;
                TrackDisposable(instance, onRelease);
            }
        }

        internal void TrackDisposable(object instance, Action<object> onRelease = null)
        {
            _cleanupService.AddIfDisposable(instance, onRelease);
        }

        internal void InitializeFromDto(ScopeConfig scopeConfig)
        {
            foreach (BindingConfig exactBinding in scopeConfig.ExactBindings)
            {
                (Type key, Registration value) = DeserializeRegistration(exactBinding);
                _registry.AddExactRegistration(key, value);
            }

            foreach (BindingConfig openConfig in scopeConfig.OpenGenericBindings)
            {
                (Type key, Registration value) = DeserializeRegistration(openConfig);
                _registry.AddOpenGenericRegistration(key, value);
            }

            foreach (BindingConfig conditionalConfig in scopeConfig.ConditionalBindings)
            {
                object id = string.IsNullOrEmpty(conditionalConfig.Id)
                    ? (string.IsNullOrEmpty(conditionalConfig.Condition)
                        ? null
                        : Type.GetType(conditionalConfig.Condition))
                    : JsonSerializer.Deserialize<object>(conditionalConfig.Id);
                (Type key, Registration value) = DeserializeRegistration(conditionalConfig);
                _registry.AddConditionalRegistration(key, id, value);
            }

            foreach (DecoratorConfig decoratorConfig in scopeConfig.DecoratorBindings)
            {
                DecoratorRegistration decoratorRegistration = new DecoratorRegistration(
                    Type.GetType(decoratorConfig.InterfaceType),
                    Type.GetType(decoratorConfig.DecoratorType),
                    decoratorConfig.Order,
                    Enum.Parse<LifeTime>(decoratorConfig.Lifetime),
                    null,
                    null);
                _registry.AddDecorator(decoratorRegistration);
            }

            if (scopeConfig.Childrens.Count > 0)
                foreach (ScopeConfig child in scopeConfig.Childrens)
                    CreateScope(child);
        }

        private (Type Key, Registration Value) DeserializeRegistration(BindingConfig bindingConfig)
        {
            Type type = Type.GetType(bindingConfig.ImplementationType);

            return (Type.GetType(bindingConfig.InterfaceType),
                new Registration(
                    type,
                    Enum.Parse<LifeTime>(bindingConfig.Lifetime),
                    null,
                    JsonSerializer.Deserialize(bindingConfig.InstanceJson, type),
                    bindingConfig.ArgumentsJson.Select(a => JsonSerializer.Deserialize<object>(a)).ToArray()));
        }

        private void CreateScope(ScopeConfig scopeConfig)
        {
            Scope child = new Scope(this, scopeConfig);

            lock (_childrens)
                _childrens.Add(child);
        }

        private static void AddExactDecoratorRegistrations(Type interfaceType, List<DecoratorRegistration> registrations, List<Scope> scopes)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = scopes[i];

                if (scope._registry.DecoratorBindings.TryGetValue(interfaceType, out List<DecoratorRegistration> inner))
                    registrations.AddRange(inner);
            }
        }

        private static void AddGenericDecoratorRegistrations(Type interfaceType, List<DecoratorRegistration> registrations, List<Scope> scopes)
        {
            Type openDeginition = interfaceType.GetGenericTypeDefinition();
            Type[] arguments = interfaceType.GetGenericArguments();

            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = scopes[i];

                if (scope._registry.DecoratorBindings.TryGetValue(openDeginition, out List<DecoratorRegistration> inner) == false)
                    continue;

                foreach (DecoratorRegistration registration in inner)
                {
                    Type closedDecorator = registration.DecoratorType.IsGenericTypeDefinition
                        ? registration.DecoratorType.MakeGenericType(arguments)
                        : registration.DecoratorType;
                    registrations.Add(new DecoratorRegistration(interfaceType, closedDecorator, registration.Order, registration.Lifetime, null, null));
                }
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

        internal sealed class Serializer
        {
            public string Serialize(Scope container)
            {
                return JsonSerializer.Serialize(GetConfig(container));
            }

            public DIContainer Deserialize(string json)
            {
                ScopeConfig config = JsonSerializer.Deserialize<ScopeConfig>(json);

                return new DIContainer(config);
            }

            private ScopeConfig GetConfig(Scope scope)
            {
                ScopeConfig scopeConfig = new ScopeConfig();

                foreach (KeyValuePair<Type, Registration> registration in scope._registry.ExactBindings)
                    scopeConfig.ExactBindings.Add(SerializeRegistration(registration.Key, registration.Value, null, null));

                foreach (KeyValuePair<Type, Registration> registration in scope._registry.OpenGenericBindings)
                    scopeConfig.OpenGenericBindings.Add(SerializeRegistration(registration.Key, registration.Value, null, null));

                foreach (KeyValuePair<ConditionalKey, Registration> registration in scope._registry.ConditionalBindings)
                {
                    scopeConfig.ConditionalBindings.Add(SerializeRegistration(registration.Key.interfaceType, registration.Value,
                        registration.Key.id as Type,
                        registration.Key.id is Type ? null : registration.Key.id));
                }

                foreach (KeyValuePair<Type, List<DecoratorRegistration>> registration in scope._registry.DecoratorBindings)
                    foreach (DecoratorRegistration decoratorRegistration in registration.Value)
                        scopeConfig.DecoratorBindings.Add(SerializeRegistrationDecorator(decoratorRegistration));

                if (scope._childrens.Count > 0)
                    foreach (Scope child in scope._childrens)
                        scopeConfig.Childrens.Add(GetConfig(child));

                return scopeConfig;
            }

            private BindingConfig SerializeRegistration(Type interfaceType, Registration registration, Type condition, object id)
            {
                return new BindingConfig()
                {
                    InterfaceType = interfaceType.AssemblyQualifiedName,
                    ImplementationType = registration.Implementation?.AssemblyQualifiedName ?? string.Empty,
                    Lifetime = registration.Lifetime.ToString(),
                    InstanceJson = JsonSerializer.Serialize(registration.Instance),
                    ArgumentsJson = registration.Arguments.Select(a => JsonSerializer.Serialize(a)).ToList(),
                    Id = id == null ? string.Empty : JsonSerializer.Serialize(id),
                    Condition = condition == null ? string.Empty : condition.AssemblyQualifiedName,
                };
            }

            private DecoratorConfig SerializeRegistrationDecorator(DecoratorRegistration registration)
            {
                return new DecoratorConfig()
                {
                    InterfaceType = registration.InterfaceType.AssemblyQualifiedName,
                    DecoratorType = registration.DecoratorType?.AssemblyQualifiedName,
                    Lifetime = registration.Lifetime.ToString(),
                    Order = registration.Order ?? 0,
                };
            }
        }
    }
}
