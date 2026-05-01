using System;
using SimplEnteiner.Core.Binder.Implementations;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.InstallerService.Interfaces;
using SimplEnteiner.Core.RegistrationService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core
{
    internal class BindExample : IInstaller
    {
        public void Install(IScope target)
        {
            // Generic version with explicit Apply
            target
                .Bind<IBindingLifetime>()
                .To<BindingLifetime>()
                .AsSingle()
                .WithId("test1")
                .WhenInjectedInto<Registry>()
                .WithArguments("123", 1234)
                .Apply();

            // Non-Generic version with implicit Apply
            target
                .Bind(typeof(IBindingOptions))
                .To(typeof(BindingOptions))
                .AsScoped()
                .WithId("test2")
                .WhenInjectedInto(typeof(Registry))
                .WithArguments("123", 1234);

            // Simple version
            target
                .Bind<BindingOptions>()
                .ToSelf()
                .AsTransient()
                .Apply();

            // Invalid version
            target
                .Bind<IBindingOptions>()
                .ToSelf()
                .AsCached()
                .Apply();

            // Decorate generic version with 3 decorators
            target
                .Decorate<IBindingOptions>()
                .With<BindingOptions>(1)
                .With<BindingOptions>()
                .With<BindingOptions>(-100)
                .Apply();

            // Decorate non-generic version with 3 decorators
            target
                .Decorate(typeof(IBindingOptions))
                .With(typeof(BindingOptions), 1)
                .With(typeof(BindingOptions))
                .With(typeof(BindingOptions), -100)
                .Apply();
        }
    }

    internal static class ProgramExample
    {
        public static void Main(string[] args)
        {
            DIContainer container = new DIContainer();

            container.ScanAndInstall();
            container.Build();
        }
    }
}
