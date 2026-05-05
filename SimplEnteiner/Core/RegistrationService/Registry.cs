using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplEnteiner.Analysis;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.Lifecycle;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.RegistrationService
{
    internal class Registry
    {
        private readonly Dictionary<Type, Registration> _exactBindings;
        private readonly Dictionary<Type, Registration> _openGenericBindings;
        private readonly Dictionary<Type, List<DecoratorRegistration>> _decoratorBindings;
        private readonly Dictionary<ConditionalKey, Registration> _conditionalBindings;

        public Registry()
        {
            _exactBindings = new Dictionary<Type, Registration>();
            _openGenericBindings = new Dictionary<Type, Registration>();
            _decoratorBindings = new Dictionary<Type, List<DecoratorRegistration>>();
            _conditionalBindings = new Dictionary<ConditionalKey, Registration>();
        }

        public IReadOnlyDictionary<Type, Registration> ExactBindings => _exactBindings;
        public IReadOnlyDictionary<Type, Registration> OpenGenericBindings => _openGenericBindings;
        public IReadOnlyDictionary<ConditionalKey, Registration> ConditionalBindings => _conditionalBindings;
        public IReadOnlyDictionary<Type, List<DecoratorRegistration>> DecoratorBindings => _decoratorBindings;

        public void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute)
        {
            Dictionary<Type, Type> allExact = new Dictionary<Type, Type>();

            foreach (KeyValuePair<Type, Registration> pair in _exactBindings)
                allExact[pair.Key] = pair.Value.Implementation;

            HashSet<Type> reachable = ReachabilityAnalyzer.Instance.ComputeReachability(roots, allExact, injectAttribute);

            List<Type> unreachable = _exactBindings.Keys.Except(reachable).ToList();
            List<Type> missing = reachable.Where(t => (allExact.ContainsKey(t) == false) && (t.IsConcreteClass() == false)).ToList();

            string message = string.Empty;

            if (unreachable.Count > 0)
                message += $"Unreachable services: {string.Join(", ", unreachable)}. ";

            if (missing.Count > 0)
                message += $"\nMissing bindings for: {string.Join(", ", missing)}";

            if (string.IsNullOrEmpty(message) == false)
                throw new InvalidOperationException(message);
        }

        internal void Add(BindingBuilderInternal builder)
        {
            Validate(builder);

            Registration registration = CreateRegistration(builder);

            if (builder.ConditionType != null || builder.Id != null)
            {
                AddConditionalRegistration(builder.InterfaceType, builder.Id ?? builder.ConditionType, registration);
            }
            else if (builder.InterfaceType.IsGenericTypeDefinition)
            {
                AddOpenGenericRegistration(builder.InterfaceType, new Registration(builder.ImplementationType ?? builder.InterfaceType, builder.LifeTime, null, null));
            }
            else
            {
                AddExactRegistration(builder.InterfaceType, registration);
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

        internal void AddDecorator(DecoratorRegistration decoratorRegistration)
        {
            ValidateDecorator(decoratorRegistration);

            Type interfaceType = decoratorRegistration.InterfaceType;

            if (_decoratorBindings.TryGetValue(interfaceType, out List<DecoratorRegistration> decorators) == false)
            {
                decorators = new List<DecoratorRegistration>();
                _decoratorBindings[interfaceType] = decorators;
            }

            decoratorRegistration.Order ??= decorators.Count == 0 ? 0 : decorators[^1].Order + 1;
            
            int insertIndex = decorators.FindBinaryIndexMoreThan(decoratorRegistration, d => d.Order.Value);
            decorators.Insert(insertIndex, decoratorRegistration);
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

            ValidateFirstStep(interfaceType, implementationType);

            ConstructorInfo ctor = implementationType.GetInjectableConstructor(injectAttribute)
                ?? throw new ArgumentException($"No public constructor found for {implementationType}.");

            ValidateSecondStep(interfaceType, implementationType);
        }

        private static void ValidateDecorator(DecoratorRegistration registration)
        {
            Type interfaceType = registration.InterfaceType;
            Type decoratorType = registration.DecoratorType;

            if (decoratorType.IsGenericTypeDefinition)
            {
                if (decoratorType.IsAssignableToGenericTypeDefinition(interfaceType) == false)
                    throw new InvalidOperationException($"Open generic decorator {decoratorType} must implement {interfaceType}.");
            }
            else
            {
                ValidateFirstStep(interfaceType, decoratorType);

                ConstructorInfo constructor = decoratorType.GetInjectableConstructor(Constants.InjectAttributeType)
                    ?? throw new ArgumentException($"No public constructor found for {decoratorType}.");
                bool hasDecoratorParameter = constructor.GetParameters().Any(p => interfaceType.IsAssignableFrom(p.ParameterType));

                if (hasDecoratorParameter == false)
                    throw new InvalidOperationException($"Decorator {decoratorType} must have a constructor parameter assignable to {interfaceType}.");

                ValidateSecondStep(interfaceType, decoratorType);
            }

        }

        private static void ValidateFirstStep(Type interfaceType, Type implementationType)
        {
            if (implementationType.IsConcreteClass(isIgnoreGeneratedType: true) == false)
                throw new ArgumentException($"Implementation {implementationType} is not a concrete class.");

            if (IsCompatible(interfaceType, implementationType) == false)
                throw new ArgumentException($"{implementationType} is not assignable to {interfaceType}.");
        }

        private static void ValidateSecondStep(Type interfaceType, Type implementationType)
        {
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
            object[] arguments = builder.Arguments?.ToArray() ?? Array.Empty<object>();

            if (instance != null)
                lifeTime = LifeTime.Singleton;

            return new Registration(implementationType, lifeTime, factory, instance, arguments);
        }
    }
}
