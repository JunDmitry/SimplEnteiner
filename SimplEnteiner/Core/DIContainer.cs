using System;
using System.Collections.Generic;
using System.Linq;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.RegistrationService;
using SimplEnteiner.Core.ResolverService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core
{
    public class DIContainer : IScope
    {
        private readonly Registry _registry;
        private readonly List<BindingBuilderInternal> _pendingBindings;
        private readonly Scope _rootScope;
        private readonly IResolver _resolver;

        public DIContainer()
        {
            _registry = new Registry();
            _resolver = new Resolver(_registry);
            _pendingBindings = new List<BindingBuilderInternal>();
            _rootScope = new Scope((t, s) => _resolver.Resolve(t, s));
        }

        public TService Resolve<TService>()
        {
            return _rootScope.Resolve<TService>();
        }

        public object Resolve(Type type)
        {
            return _rootScope.Resolve(type);
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

            _registry.ValidateAll();
        }

        internal void Register(BindingBuilderInternal builder)
        {
            if (RegisterWithoutRemove(builder) == false)
                return;

            lock (_pendingBindings)
                _pendingBindings.Remove(builder);
        }

        private bool RegisterWithoutRemove(BindingBuilderInternal builder)
        {
            if (builder.IsRegistered)
                return false;

            builder.ExecuteAllStages();
            _registry.Add(builder);
            builder.MarkRegistered();

            return true;
        }
    }
}
