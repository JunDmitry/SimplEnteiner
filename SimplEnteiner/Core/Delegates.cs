using System;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Core
{
    public delegate object ResolverFunc(Type interfaceType, Scope scope, object id = null);
}
