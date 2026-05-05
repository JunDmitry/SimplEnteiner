using System;
using Microsoft.Extensions.DependencyInjection;
using SimplEnteiner.Core;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Integrations.MS_DI
{
    public static class Extensions
    {
        public static IServiceCollection AddSimplEnteiner(this IServiceCollection services, Action<DIContainer> configure)
        {
            DIContainer container = new DIContainer();
            configure.ThrowIfArgumentNull().Invoke(container);
            container.Build();
            
            services.AddSingleton<IServiceProvider>(new SimplEnteinerServiceProvider(container));
            services.AddSingleton<IServiceScopeFactory>(new SimplEnteinerServiceProvider(container));

            return services;
        }
    }
}
