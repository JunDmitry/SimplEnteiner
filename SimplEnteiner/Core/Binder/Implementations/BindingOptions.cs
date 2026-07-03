using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingOptions<TInterface> : IBindingOptions<TInterface>
    {
        private readonly BindingBuilder _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingOptions(BindingBuilder bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public void Apply()
        {
            _container.Register(_bindingBuilderInternal);
        }

        public IBindingOptions<TInterface> OnActivation(Action<TInterface> onActivation)
        {
            onActivation.ThrowIfArgumentNull();
            _bindingBuilderInternal.SetOnActivation(obj => onActivation((TInterface)obj));

            return this;
        }

        public IBindingOptions<TInterface> OnRelease(Action<TInterface> onRelease)
        {
            onRelease.ThrowIfArgumentNull();
            _bindingBuilderInternal.SetOnRelease(obj => onRelease((TInterface)obj));

            return this;
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
        private readonly BindingBuilder _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingOptions(BindingBuilder bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public void Apply()
        {
            _container.Register(_bindingBuilderInternal);
        }

        public IBindingOptions OnActivation(Action<object> onActivation)
        {
            onActivation.ThrowIfArgumentNull();
            _bindingBuilderInternal.SetOnActivation(onActivation);

            return this;
        }

        public IBindingOptions OnRelease(Action<object> onRelease)
        {
            onRelease.ThrowIfArgumentNull();
            _bindingBuilderInternal.SetOnRelease(onRelease);

            return this;
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
