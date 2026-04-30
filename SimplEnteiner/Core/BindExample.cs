using System;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.RegistrationService;

namespace SimplEnteiner.Core
{
    internal class BindExample
    {
        public void Install(DIContainer container)
        {
            // Generic version
            container
                .Bind<IBindingLifetime>()
                .To<BindingLifetime>()
                .AsSingle()
                .WithId("test1")
                .WhenInjectedInto<Registry>()
                .WithArguments("123", 1234)
                .Apply();

            // Non-Generic version
            container
                .Bind(typeof(IBindingOptions))
                .To(typeof(BindingOptions))
                .AsScoped()
                .WithId("test2")
                .WhenInjectedInto(typeof(Registry))
                .WithArguments("123", 1234)
                .Apply();

            // Simple version
            container
                .Bind<BindingOptions>()
                .ToSelf()
                .AsTransient()
                .Apply();

            // Invalid version
            container
                .Bind<IBindingOptions>()
                .ToSelf()
                .AsCached()
                .Apply();
        }
    }
}
