using System;
using System.Reflection;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.ConventionBinding.Interfaces
{
    public interface IConventionBuilder
    {
        IConventionBuilder If(Func<Type, bool> predicate);
        IConventionBuilder FromAssembly(Assembly assembly);
        IConventionBuilder FromAssemblies(params Assembly[] assemblies);
        IConventionBuilder InNamespace(string namespacePrefix);
        IConventionBuilder WithAttribute<TAttribute>() where TAttribute : Attribute;
        IConventionBuilder BindInterfaces();
        IConventionBuilder BindSelf();
        IConventionBuilder As(LifeTime lifetime);
        void Configure(Action<IBinder> registration);
    }
}
