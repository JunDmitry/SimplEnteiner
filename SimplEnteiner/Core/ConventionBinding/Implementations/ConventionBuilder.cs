using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.ConventionBinding.Interfaces;
using SimplEnteiner.Core.Lifecycle;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ConventionBinding.Implementations
{
    public class ConventionBuilder : IConventionBuilder
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<Func<Type, bool>> _ifPredicates = new List<Func<Type, bool>>();
        private readonly IScope _scope;

        public ConventionBuilder(IScope scope)
        {
            _scope = scope;
        }

        internal IReadOnlyList<Assembly> Assemblies => _assemblies.AsReadOnly();
        internal string NamespacePrefix { get; private set; } = string.Empty;
        internal Type AttributeType { get; private set; }
        internal IReadOnlyList<Func<Type, bool>> IfPredicates => _ifPredicates.AsReadOnly();
        internal ConventionBindType BindType { get; private set; } = ConventionBindType.None;
        internal LifeTime Lifetime { get; private set; } = LifeTime.Transient;
        internal Action<IBinder> ConfigureBinder { get; private set; }

        public IConventionBuilder As(LifeTime lifetime)
        {
            Lifetime = lifetime;

            return this;
        }

        public IConventionBuilder BindInterfaces()
        {
            BindType |= ConventionBindType.Interface;

            return this;
        }

        public IConventionBuilder BindSelf()
        {
            BindType |= ConventionBindType.Self;

            return this;
        }

        public void Configure(Action<IBinder> registration)
        {
            ConfigureBinder = registration.ThrowIfArgumentNull();
        }

        public IConventionBuilder FromAssemblies(params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies.ThrowIfArgumentNull())
                FromAssembly(assembly);

            return this;
        }

        public IConventionBuilder FromAssembly(Assembly assembly)
        {
            assembly.ThrowIfArgumentNull();

            if (_assemblies.Contains(assembly))
                throw new ArgumentException($"This assembly {assembly} already contains in {nameof(ConventionBuilder)}.");

            _assemblies.Add(assembly);

            return this;
        }

        public IConventionBuilder If(Func<Type, bool> predicate)
        {
            predicate.ThrowIfArgumentNull();

            _ifPredicates.Add(predicate);

            return this;
        }

        public IConventionBuilder InNamespace(string namespacePrefix)
        {
            namespacePrefix.ThrowIfArgumentNull();

            NamespacePrefix = namespacePrefix;

            return this;
        }

        public IConventionBuilder WithAttribute<TAttribute>() where TAttribute : Attribute
        {
            AttributeType = typeof(TAttribute);

            return this;
        }

        internal void Build()
        {
            if (_assemblies.Count == 0)
                _assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            string namespaceFilterPrefix = NamespacePrefix ?? string.Empty;
            List<Type> typesForBind = new List<Type>();

            foreach (Assembly assembly in _assemblies)
            {
                foreach (Type type in assembly.GetLoadableTypes())
                {
                    if (type.Namespace.StartsWith(namespaceFilterPrefix) == false)
                        continue;

                    if (AttributeType != null && type.GetCustomAttribute(AttributeType) == null)
                        continue;

                    if (_ifPredicates.Any(p => p(type) == false))
                        continue;

                    typesForBind.Add(type);
                }
            }

            foreach (Type type in typesForBind)
                Bind(type);

            ConfigureBinder?.Invoke(_scope);
        }

        private void Bind(Type type)
        {
            if ((BindType & ConventionBindType.Interface) != 0)
            {
                Type[] interfaces = type.GetInterfaces();

                foreach (Type iface in interfaces)
                {
                    BindLifetime(_scope.Bind(iface).To(type), Lifetime).Apply();
                }
            }

            if ((BindType & ConventionBindType.Self) != 0)
                BindLifetime(_scope.Bind(type).ToSelf(), Lifetime).Apply();
        }

        private IBindingOptions BindLifetime(IBindingLifetime bindingLifetime, LifeTime lifetime)
        {
            return lifetime switch
            {
                LifeTime.Transient => bindingLifetime.AsTransient(),
                LifeTime.Cached => bindingLifetime.AsCached(),
                LifeTime.Scoped => bindingLifetime.AsScoped(),
                LifeTime.Singleton => bindingLifetime.AsSingle(),
                _ => throw new NotImplementedException()
            };
        } 
    }

    [Flags]
    internal enum ConventionBindType
    {
        None = 0,
        Interface = 1 << 0,
        Self = 1 << 1,
    }
}
