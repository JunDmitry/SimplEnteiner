using System;
using Microsoft.Extensions.DependencyInjection;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Integrations.MS_DI
{
    public class SimplEnteinerServiceProvider : IServiceProvider, ISupportRequiredService, IServiceScopeFactory
    {
        private readonly IScope _container;

        public SimplEnteinerServiceProvider(IScope container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return _container.Resolve(serviceType)
                ?? throw new InvalidOperationException($"Service {serviceType} not registered.");
        }

        public IServiceScope CreateScope()
        {
            IScope scope = _container.CreateScope();

            return new SimplEnteinerServiceScope(scope);
        }
    }
}
