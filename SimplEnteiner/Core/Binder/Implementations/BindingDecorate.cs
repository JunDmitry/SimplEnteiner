using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingDecorate<TInterface> : IBindingDecorate<TInterface>
    {
        private readonly BindingBuilderInternal _bindingBuilder;
        private readonly IBindingTarget _target;

        public BindingDecorate(BindingBuilderInternal bindingBuilder, IBindingTarget target)
        {
            _bindingBuilder = bindingBuilder;
            _target = target;
        }

        public IBindingDecorate<TInterface> With<TImplementation>(int? order = null) where TImplementation : TInterface
        {
            _bindingBuilder.AddDecorator(typeof(TImplementation), order);

            return this;
        }

        public void Apply()
        {
            if (_bindingBuilder.HasDecorators == false)
                throw new InvalidOperationException($"Invalid operation: decorated service does not have added decorators!");

            _target.Register(_bindingBuilder);
        }
    }

    internal class BindingDecorate : IBindingDecorate
    {
        private readonly BindingBuilderInternal _bindingBuilder;
        private readonly IBindingTarget _target;

        public BindingDecorate(BindingBuilderInternal bindingBuilder, IBindingTarget target)
        {
            _bindingBuilder = bindingBuilder;
            _target = target;
        }

        public IBindingDecorate With(Type implementationType, int? order = null)
        {
            _bindingBuilder.AddDecorator(implementationType.ThrowIfArgumentNull(), order);

            return this;
        }

        public void Apply()
        {
            if (_bindingBuilder.HasDecorators == false)
                throw new InvalidOperationException($"Invalid operation: decorated service does not have added decorators!");

            _target.Register(_bindingBuilder);
        }
    }
}
