using System;
using System.Collections.Generic;
using SimplEnteiner.Core.Binder;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Core.RegistrationService
{
    public interface IRegistry
    {
        IReadOnlyDictionary<ConditionalKey, Registration> ConditionalBindings { get; }
        IReadOnlyDictionary<Type, List<DecoratorRegistration>> DecoratorBindings { get; }
        IReadOnlyDictionary<Type, Registration> ExactBindings { get; }
        IReadOnlyDictionary<Type, Registration> OpenGenericBindings { get; }

        void Add(BindingBuilder bindingBuilder);
        void AddConditionalRegistration(Type key, object id, Registration value);
        void AddDecorator(DecoratorRegistration registration);
        void AddExactRegistration(Type key, Registration value);
        void AddOpenGenericRegistration(Type key, Registration value);
        void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute);
        bool CanResolveAllDependencies(Type type, Type injectAttribute);
        bool CanResolveGeneric(Type interfaceType);
        void ValidateAll();
    }
}