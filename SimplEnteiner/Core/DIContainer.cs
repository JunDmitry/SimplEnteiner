using System;
using System.Collections.Generic;
using System.Linq;
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
using SimplEnteiner.Core.RegistrationService.Factory;
using SimplEnteiner.Core.ResolverService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core
{
    public class DIContainer : IScope, IBindingTarget
    {
        private readonly List<BindingBuilder> _pendingBindings;
        private readonly IResolver _resolver;
        private readonly Serializer _serializer = new Serializer();

        private Scope _rootScope;

        public DIContainer()
        {
            _resolver = new Resolver();
            _pendingBindings = new List<BindingBuilder>();
            _rootScope = new Scope(ConfigureConfig);
        }

        internal DIContainer(ScopeConfig rootScopeConfig)
        {
            _resolver = new Resolver();
            _pendingBindings = new List<BindingBuilder>();
            _rootScope = new Scope(ConfigureConfig, rootScopeConfig);
        }

        public IScope Parent => _rootScope.Parent;

        public IRegistry Registry => ((IScope)_rootScope).Registry;

        public bool IsRoot => ((IScope)_rootScope).IsRoot;

        public TService Resolve<TService>()
        {
            return _rootScope.Resolve<TService>();
        }

        public object Resolve(Type type)
        {
            return _rootScope.Resolve(type);
        }

        public async Task<TService> ResolveAsync<TService>()
        {
            return await _rootScope.ResolveAsync<TService>();
        }

        public async Task<object> ResolveAsync(Type type)
        {
            return await _rootScope.ResolveAsync(type);
        }

        public object Resolve(Type interfaceType, object id)
        {
            return _rootScope.Resolve(interfaceType, id);
        }

        public T Resolve<T>(object id)
        {
            return (T)_rootScope.Resolve(typeof(T), id);
        }

        public async Task<object> ResolveAsync(Type interfaceType, object id)
        {
            return await _rootScope.ResolveAsync(interfaceType, id);
        }

        public async Task<T> ResolveAsync<T>(object id)
        {
            return await _rootScope.ResolveAsync<T>(id);
        }

        public IScope[] GetChildrens()
        {
            return _rootScope.GetChildrens();
        }

        public void GetChildrens(List<IScope> results)
        {
            _rootScope.GetChildrens(results);
        }

        public IScope CreateScope()
        {
            return _rootScope.CreateScope();
        }

        public void Dispose()
        {
            _rootScope.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _rootScope.DisposeAsync();
        }

        public IBindingTo<TService> Bind<TService>()
        {
            BindingBuilder bindingBuilder = new BindingBuilder(typeof(TService));

            lock (_pendingBindings)
                _pendingBindings.Add(bindingBuilder);

            return new BindingTo<TService>(bindingBuilder, this);
        }

        public IBindingTo Bind(Type serviceType)
        {
            serviceType.ThrowIfArgumentNull();

            BindingBuilder bindingBuilder = new BindingBuilder(serviceType);

            lock (_pendingBindings)
                _pendingBindings.Add(bindingBuilder);

            return new BindingTo(bindingBuilder, this);
        }

        public void BindConvention(Action<IConventionBuilder> configure)
        {
            ConventionBuilder builder = new ConventionBuilder(this);
            configure.ThrowIfArgumentNull().Invoke(builder);

            builder.Build();
            BuildPendings();
        }

        public IBindingDecorate<TService> Decorate<TService>()
        {
            BindingBuilder builderInternal = new BindingBuilder(typeof(TService));

            return new BindingDecorate<TService>(builderInternal, this);
        }

        public IBindingDecorate Decorate(Type interfaceType)
        {
            interfaceType.ThrowIfArgumentNull();
            BindingBuilder builderInternal = new BindingBuilder(interfaceType);

            return new BindingDecorate(builderInternal, this);
        }

        public virtual void Install(IInstaller installer)
        {
            installer.ThrowIfArgumentNull().Install(this);
        }

        public void Build()
        {
            BuildPendings();

            _rootScope.Build();
        }

        public string ExportConfiguration()
        {
            return _serializer.Serialize(this);
        }

        public void ImportConfiguration(string jsonConfiguration)
        {
            _rootScope?.Dispose();
            _pendingBindings.Clear();

            _rootScope = new Scope(ConfigureConfig);
            _rootScope.InitializeFromDto(_serializer.DeserializeInternal(jsonConfiguration));
            Build();
        }

        public void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute)
        {
            _rootScope.AnalyzeReachability(roots, injectAttribute);
        }

        void IBindingTarget.Register(BindingBuilder builder)
        {
            if (RegisterWithoutRemove(builder) == false)
                return;

            lock (_pendingBindings)
                _pendingBindings.Remove(builder);
        }

        void IBindingTarget.RegisterDecorator(BindingBuilder bindingBuilder)
        {
            ((IBindingTarget)_rootScope).RegisterDecorator(bindingBuilder);
        }

        private void ConfigureConfig(ScopeCreationConfig config)
        {
            config.Resolver = _resolver;
            config.ScopeFactory = new ScopeFactory.DefaultScopeFactory();
            config.RegistryFactory = new RegistryFactory();
            config.SingletonRepository = new RepositoryService.RepositoryService(new CleanupService());
        }

        private void BuildPendings()
        {
            List<BindingBuilder> pendingBindingsCopy;

            lock (_pendingBindings)
                pendingBindingsCopy = _pendingBindings.ToList();

            foreach (BindingBuilder bindingBuilder in pendingBindingsCopy)
            {
                RegisterWithoutRemove(bindingBuilder);
            }

            lock (_pendingBindings)
                _pendingBindings.Clear();
        }

        private bool RegisterWithoutRemove(BindingBuilder builder)
        {
            if (builder.IsRegistered)
                return false;

            builder.ExecuteAllStages();
            _rootScope.AddRegister(builder);
            builder.MarkRegistered();

            return true;
        }

        public sealed class Serializer
        {
            private readonly Scope.Serializer _serializer = new Scope.Serializer();

            public string Serialize(DIContainer container)
            {
                return _serializer.Serialize(container._rootScope);
            }

            public DIContainer Deserialize(string json)
            {
                return _serializer.Deserialize(json);
            }

            internal ScopeConfig DeserializeInternal(string json)
            {
                return JsonSerializer.Deserialize<ScopeConfig>(json);
            }
        }

        public void RemoveChildren(IScope child)
        {
            ((IScope)_rootScope).RemoveChildren(child);
        }
    }
}
