using System;

namespace SimplEnteiner.Core.ScopeFeature
{
    public interface IScope : IDisposable
    {
        IScope CreateScope();
        object Resolve(Type type);
        T Resolve<T>();
    }
}
