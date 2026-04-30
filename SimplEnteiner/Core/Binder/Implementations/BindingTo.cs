using System;
using SimplEnteiner.Core.Binder.Interfaces;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingTo<TInterface> : IBindingTo<TInterface>
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingTo(BindingBuilderInternal bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public IBindingLifetime<TInterface> To<TImplementation>() where TImplementation : TInterface
        {
            _bindingBuilderInternal.SetImplementation(typeof(TImplementation));
            return new BindingLifetime<TInterface>(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime<TInterface> ToInstance(TInterface instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _bindingBuilderInternal.SetInstance(instance);
            return new BindingLifetime<TInterface>(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime<TInterface> ToMethod(Func<TInterface> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _bindingBuilderInternal.SetFactoryMethod(() => factory());
            return new BindingLifetime<TInterface>(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime<TInterface> ToSelf()
        {
            _bindingBuilderInternal.SetImplementation(typeof(TInterface));
            return new BindingLifetime<TInterface>(_bindingBuilderInternal, _container);
        }
    }

    internal class BindingTo : IBindingTo
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingTo(BindingBuilderInternal bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public IBindingLifetime To(Type implementation)
        {
            _bindingBuilderInternal.SetImplementation(implementation);
            return new BindingLifetime(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime ToInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _bindingBuilderInternal.SetInstance(instance);
            return new BindingLifetime(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime ToMethod(Func<object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _bindingBuilderInternal.SetFactoryMethod(factory);
            return new BindingLifetime(_bindingBuilderInternal, _container);
        }

        public IBindingLifetime ToSelf()
        {
            _bindingBuilderInternal.SetImplementation(_bindingBuilderInternal.InterfaceType);
            return new BindingLifetime(_bindingBuilderInternal, _container);
        }
    }
}
