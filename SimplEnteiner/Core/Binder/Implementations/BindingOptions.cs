using System;
using SimplEnteiner.Core.Binder.Interfaces;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingOptions<TInterface> : IBindingOptions<TInterface>
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly DIContainer _container;

        public BindingOptions(BindingBuilderInternal bindingBuilderInternal, DIContainer container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public void Apply()
        {
            _container.Register(_bindingBuilderInternal);
        }

        public IBindingOptions<TInterface> WhenInjectedInto<T>()
        {
            _bindingBuilderInternal.SetCondition<T>();

            return this;
        }

        public IBindingOptions<TInterface> WithArguments(params object[] args)
        {
            _bindingBuilderInternal.Arguments.AddRange(args);

            return this;
        }

        public IBindingOptions<TInterface> WithId(object id)
        {
            _bindingBuilderInternal.SetId(id);

            return this;
        }
    }

    internal class BindingOptions : IBindingOptions
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly DIContainer _container;

        public BindingOptions(BindingBuilderInternal bindingBuilderInternal, DIContainer container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public void Apply()
        {
            _container.Register(_bindingBuilderInternal);
        }

        public IBindingOptions WhenInjectedInto(Type type)
        {
            _bindingBuilderInternal.SetCondition(type);

            return this;
        }

        public IBindingOptions WithArguments(params object[] args)
        {
            _bindingBuilderInternal.Arguments.AddRange(args);

            return this;
        }

        public IBindingOptions WithId(object id)
        {
            _bindingBuilderInternal.SetId(id);

            return this;
        }
    }
}
