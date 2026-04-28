using System;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.Registration;

namespace SimplEnteiner.Core
{
    public class DIContainer
    {
        private readonly Registry _registry;
        private readonly Type _injectAttributeType = Constants.InjectAttributeType;

        public DIContainer()
        {
            _registry = new Registry();
        }

        public IBindingTo<TService> Bind<TService>()
        {
            BindingBuilderInternal bindingBuilder = new BindingBuilderInternal(typeof(TService));

            return new BindingTo<TService>(bindingBuilder, this);
        }

        public IBindingTo Bind(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            BindingBuilderInternal bindingBuilder = new BindingBuilderInternal(serviceType);

            return new BindingTo(bindingBuilder, this);
        }

        public TService Resolve<TService>()
        {
            return (TService) Resolve(typeof(TService));
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        internal void Register(BindingBuilderInternal builder)
        {
            // TODO: Validation and adding into registry
            _registry.Add(builder);
        }
    }
}
