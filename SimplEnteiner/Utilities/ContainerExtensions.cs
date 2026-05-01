using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplEnteiner.Core;
using SimplEnteiner.Core.InstallerService.Interfaces;
using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Utilities
{
    public static class ContainerExtensions
    {
        public static void ScanAndInstall(this DIContainer container, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type installerType = typeof(IInstaller);
            IEnumerable<Type> installerTypes = GetInstallerTypes(assemblies, installerType);

            foreach (Type type in installerTypes)
            {
                IInstaller installer = (IInstaller)container.Resolve(type);

                installer.Install(container);
            }
        }

        public static void ScanAndInstall(this IScope scope, params Assembly[] assemblies)
        {
            if (scope is DIContainer container)
            {
                container.ScanAndInstall(assemblies);
                return;
            }

            if (assemblies == null || assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type installerType = typeof(IInstaller);
            IEnumerable<Type> installerTypes = GetInstallerTypes(assemblies, installerType);

            foreach (Type type in installerTypes)
            {
                IInstaller installer = (IInstaller)scope.Resolve(type);

                installer.Install(scope);
            }
        }

        private static IEnumerable<Type> GetInstallerTypes(Assembly[] assemblies, Type installerType)
        {
            return assemblies
                .SelectMany(a => a.GetLoadableTypes())
                .Where(t => installerType.IsAssignableFrom(t) && t.IsConcreteClass(isIgnoreGeneratedType: true))
                .ToList();
        }
    }
}
