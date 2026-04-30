using System;
using System.Collections.Generic;
using System.Reflection;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.LifeScope;

namespace SimplEnteiner.Core.RegistrationService
{
    internal class Registry
    {
        private readonly Dictionary<Type, Registration> _exactBindings;
        private readonly Dictionary<Type, Registration> _openGenericBindings;
        private readonly Dictionary<(Type, object), Registration> _conditionalBindings;

        public Registry()
        {
            _exactBindings = new Dictionary<Type, Registration>();
            _openGenericBindings = new Dictionary<Type, Registration>();
            _conditionalBindings = new Dictionary<(Type, object), Registration>();
        }

        public IReadOnlyDictionary<Type, Registration> ExactBindings => _exactBindings;
        public IReadOnlyDictionary<Type, Registration> OpenGenericBindings => _openGenericBindings;
        public IReadOnlyDictionary<(Type, object), Registration> ConditionalBindings => _conditionalBindings;

        internal void Add(BindingBuilderInternal builder)
        {
            Validate(builder);

            Registration registration = CreateRegistration(builder);

            if (builder.ConditionType != null || builder.Id != null)
            {
                (Type, object) key = (builder.InterfaceType, builder.Id ?? builder.ConditionType);
                _conditionalBindings[key] = registration;
            }
            else if (builder.InterfaceType.IsGenericTypeDefinition)
            {
                _openGenericBindings[builder.InterfaceType] = new Registration(builder.ImplementationType ?? builder.InterfaceType, builder.LifeTime, null, null);
            }
            else
            {
                _exactBindings[builder.InterfaceType] = registration;
            }

            // Iterative validating maybe can invalid, if not all dependency registered in current time
            //if (CanResolveAllDependencies(builder.InterfaceType, Constants.InjectAttributeType) == false)
            //    throw new InvalidOperationException($"Cannot resolve all dependencies of {builder.InterfaceType} with current registrations.");
        }

        internal void AddExactRegistration(Type interfaceType, Registration registration)
        {
            _exactBindings[interfaceType] = registration;
        }

        internal void AddOpenGenericRegistration(Type interfaceType, Registration registration)
        {
            _openGenericBindings[interfaceType] = registration;
        }

        internal void AddConditionalRegistration(Type interfaceType, object id, Registration registration)
        {
            _conditionalBindings[(interfaceType, id)] = registration;
        }

        internal bool CanResolveGeneric(Type interfaceType)
        {
            return _exactBindings.ContainsKey(interfaceType)
                || (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false)
                    && _openGenericBindings.ContainsKey(interfaceType.GetGenericTypeDefinition()));
        }

        internal bool CanResolveAllDependencies(Type type, Type injectAttribute)
        {
            return type.CanResolveAllDependencies(injectAttribute, _exactBindings, b => b.Implementation);
        }

        internal void ValidateAll()
        {
            Type injectAttribute = Constants.InjectAttributeType;

            foreach (Type interfaceType in _exactBindings.Keys)
            {
                if (interfaceType.CanResolveAllDependencies(injectAttribute, _exactBindings, t => t.Implementation) == false)
                    throw new InvalidOperationException($"Cannot resolve all dependencies of {interfaceType}.");
            }

            foreach (KeyValuePair<Type, Registration> pair in _openGenericBindings)
            {
                Type implementation = pair.Value.Implementation;

                if (implementation.IsConcreteClass(isIgnoreGeneratedType: true) == false)
                    throw new InvalidOperationException($"Open generic implementation {implementation} for {pair.Key} is not a concrete class.");

                ConstructorInfo ctor = implementation.GetInjectableConstructor(injectAttribute) 
                    ?? throw new InvalidOperationException($"Open generic implementation {implementation} has no injectable constructor.");
            }
        }

        private void Validate(BindingBuilderInternal builder)
        {
            Type injectAttribute = Constants.InjectAttributeType;
            Type interfaceType = builder.InterfaceType;
            Type implementationType = builder.ImplementationType ?? interfaceType;

            if (implementationType.IsConcreteClass(isIgnoreGeneratedType: true) == false)
                throw new ArgumentException($"Implementation {implementationType} is not a concrete class.");

            if (IsCompatible(interfaceType, implementationType) == false)
                throw new ArgumentException($"{implementationType} is not assignable to {interfaceType}.");

            ConstructorInfo ctor = implementationType.GetInjectableConstructor(injectAttribute)
                ?? throw new ArgumentException($"No public constructor found for {implementationType}.");

            if (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false))
            {
                if (implementationType.SatisfiesClosedGenericConstraints(interfaceType) == false)
                    throw new ArgumentException($"{implementationType} does not satisfy the generic constraints of {interfaceType}.");
            }
            else if (interfaceType.IsGenericTypeDefinition)
            {
                if (implementationType.IsAssignableToGenericTypeDefinition(interfaceType) == false)
                    throw new ArgumentException($"{implementationType} does not implement open generic {interfaceType}");
            }
        }

        private static bool IsCompatible(Type interfaceType, Type implementationType)
        {
            if (interfaceType.IsGenericTypeDefinition)
                return implementationType.IsAssignableToGenericTypeDefinition(interfaceType);

            return interfaceType.IsAssignableFrom(implementationType);
        }

        private Registration CreateRegistration(BindingBuilderInternal builder)
        {
            Type implementationType = builder.ImplementationType ?? builder.InterfaceType;
            ConstructorInfo constructor = implementationType.GetInjectableConstructor(Constants.InjectAttributeType);

            Func<object> factoryMethod = builder.FactoryMethod;
            Func<object[], object> factory = factoryMethod != null
                ? _ => factoryMethod()
                : constructor.GetFactoryMethod();
            object instance = builder.Instance;
            LifeTime lifeTime = builder.LifeTime;

            if (instance != null)
                lifeTime = LifeTime.Singleton;

            return new Registration(implementationType, lifeTime, factory, instance);
        }
    }
}
