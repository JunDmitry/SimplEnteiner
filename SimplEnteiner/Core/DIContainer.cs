using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;
using SimplEnteiner.Core.Lifecycle;
using SimplEnteiner.Core.ResolverService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core
{
    public class DIContainer : IScope, IBindingTarget
    {
        private readonly List<BindingBuilderInternal> _pendingBindings;
        private readonly Scope _rootScope;
        private readonly IResolver _resolver;

        public DIContainer()
        {
            _resolver = new Resolver();
            _pendingBindings = new List<BindingBuilderInternal>();
            _rootScope = new Scope((t, s, id) => _resolver.Resolve(t, s, id));
        }

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

        public IScope CreateScope()
        {
            return _rootScope.CreateScope();
        }

        public void Dispose()
        {
            _rootScope.Dispose();
        }

        public IBindingTo<TService> Bind<TService>()
        {
            BindingBuilderInternal bindingBuilder = new BindingBuilderInternal(typeof(TService));
            
            lock(_pendingBindings) 
                _pendingBindings.Add(bindingBuilder);

            return new BindingTo<TService>(bindingBuilder, this);
        }

        public IBindingTo Bind(Type serviceType)
        {
            serviceType.ThrowIfArgumentNull();

            BindingBuilderInternal bindingBuilder = new BindingBuilderInternal(serviceType);
           
            lock(_pendingBindings)
                _pendingBindings.Add(bindingBuilder);

            return new BindingTo(bindingBuilder, this);
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

        public virtual void Install(IInstaller installer)
        {
            installer.ThrowIfArgumentNull().Install(this);
        }

        public void Build()
        {
            List<BindingBuilderInternal> pendingBindingsCopy;

            lock (_pendingBindings)
                pendingBindingsCopy = _pendingBindings.ToList();

            foreach (BindingBuilderInternal bindingBuilder in pendingBindingsCopy)
            {
                RegisterWithoutRemove(bindingBuilder);
            }

            lock (_pendingBindings)
                _pendingBindings.Clear();

            _rootScope.ValidateAll();
            _rootScope.Start();
        }

        void IBindingTarget.Register(BindingBuilderInternal builder)
        {
            if (RegisterWithoutRemove(builder) == false)
                return;

            lock (_pendingBindings)
                _pendingBindings.Remove(builder);
        }

        void IBindingTarget.RegisterDecorator(BindingBuilderInternal bindingBuilder)
        {
            ((IBindingTarget)_rootScope).RegisterDecorator(bindingBuilder);
        }

        private bool RegisterWithoutRemove(BindingBuilderInternal builder)
        {
            if (builder.IsRegistered)
                return false;

            builder.ExecuteAllStages();
            _rootScope.AddRegister(builder);
            builder.MarkRegistered();

            return true;
        }
    }
}
