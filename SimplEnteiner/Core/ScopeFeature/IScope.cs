using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;
using SimplEnteiner.Core.RegistrationService;

namespace SimplEnteiner.Core.ScopeFeature
{
    public interface IScope : IDisposable, IBinder, IAsyncDisposable
    {
        IScope Parent { get; }
        IRegistry Registry { get; }
        bool IsRoot { get; }

        IScope[] GetChildrens();
        void GetChildrens(List<IScope> results);
        IScope CreateScope();
        void RemoveChildren(IScope child);

        object Resolve(Type type);
        T Resolve<T>();

        Task<object> ResolveAsync(Type type);
        Task<T> ResolveAsync<T>();

        object Resolve(Type type, object id);
        T Resolve<T>(object id);

        Task<object> ResolveAsync(Type type, object id);
        Task<T> ResolveAsync<T>(object id);

        void Install(IInstaller installer);
        void Build();
    }
}
