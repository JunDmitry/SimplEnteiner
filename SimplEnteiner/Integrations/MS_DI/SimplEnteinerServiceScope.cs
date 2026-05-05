using System;
using Microsoft.Extensions.DependencyInjection;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Integrations.MS_DI
{
    public class SimplEnteinerServiceScope : IServiceScope
    {
        public SimplEnteinerServiceScope(IScope scope)
        {
            ServiceProvider = new SimplEnteinerServiceProvider(scope);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
