using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingDecorate<TInterface> : IBindingDecorate<TInterface>
    {
        private readonly BindingBuilder _bindingBuilder;
        private readonly IBindingTarget _target;

        public BindingDecorate(BindingBuilder bindingBuilder, IBindingTarget target)
        {
            _bindingBuilder = bindingBuilder;
            _target = target;
        }

        public IBindingDecorateLifetime<TInterface> With<TImplementation>(int? order = null) where TImplementation : TInterface
        {
            _bindingBuilder.AddDecorator(typeof(TImplementation), order);

            return new BindingDecorateLifetime<TInterface>(_bindingBuilder, _target);
        }
    }

    internal class BindingDecorate : IBindingDecorate
    {
        private readonly BindingBuilder _bindingBuilder;
        private readonly IBindingTarget _target;

        public BindingDecorate(BindingBuilder bindingBuilder, IBindingTarget target)
        {
            _bindingBuilder = bindingBuilder;
            _target = target;
        }

        public IBindingDecorateLifetime With(Type implementationType, int? order = null)
        {
            Validate(implementationType);

            _bindingBuilder.AddDecorator(implementationType.ThrowIfArgumentNull(), order);

            return new BindingDecorateLifetime(_bindingBuilder, _target);
        }

        private void Validate(Type implementationType)
        {
            Type interfaceType = _bindingBuilder.InterfaceType;

            if (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false))
            {
                if ((implementationType.IsConcreteClass() == false) 
                    || implementationType.SatisfiesClosedGenericConstraints(interfaceType) == false)
                    throw new InvalidOperationException($"Invalid decorator {implementationType}. Should assignable to {interfaceType}.");
            }
            else if (interfaceType.IsGenericType)
            {
                if (implementationType.IsAssignableToGenericTypeDefinition(interfaceType) == false)
                    throw new InvalidOperationException($"Invalid decorator {implementationType}. Should assignable to {interfaceType}.");
            }
            else
            {
                if (interfaceType.IsAssignableFrom(implementationType) == false)
                    throw new InvalidOperationException($"Invalid decorator {implementationType}. Should assignable to {interfaceType}.");
            }
        }
    }
}
