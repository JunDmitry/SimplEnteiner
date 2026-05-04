using System;
using System.Threading.Tasks;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;

namespace SimplEnteiner.Core.ScopeFeature
{
    public interface IScope : IDisposable, IBinder, IAsyncDisposable
    {
        IScope CreateScope();
        object Resolve(Type type);
        T Resolve<T>();
        Task<object> ResolveAsync(Type type);
        Task<T> ResolveAsync<T>();
        object Resolve(Type type, object id);
        T Resolve<T>(object id);
        Task<object> ResolveAsync(Type type, object id);
        Task<T> ResolveAsync<T>(object id);
        void Install(IInstaller installer);
    }
}
