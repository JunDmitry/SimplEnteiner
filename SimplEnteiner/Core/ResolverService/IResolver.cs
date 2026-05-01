using System;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Core.ResolverService
{
    public interface IResolver
    {
        object Resolve(Type interfaceType, Scope scope, object id = null);
        T Resolve<T>(Scope scope, object id = null);
    }
}