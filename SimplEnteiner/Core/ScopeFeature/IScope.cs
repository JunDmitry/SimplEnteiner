using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;

namespace SimplEnteiner.Core.ScopeFeature
{
    public interface IScope : IDisposable, IBinder
    {
        IScope CreateScope();
        object Resolve(Type type);
        T Resolve<T>();
        void Install(IInstaller installer);
    }
}
