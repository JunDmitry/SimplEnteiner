using System.Collections.Concurrent;
using System.Reflection;
using SimplEnteiner;

namespace SimplEnteinerTests.TestInfrastructure
{
    /// <summary>
    /// Base test class that provides helpers to reset static domain-type cache
    /// used by TypeExtensions (FindAllAssignableFrom / FindAllNonAbstractClassAssignableFrom).
    /// </summary>
    public abstract class TypeExtensionsTestBase
    {
        private const string AssemblyCacheFieldName = "s_assembliesCache";
        private const string CacheDomainTypesFieldName = "s_cachedDomainTypes";
        private const string InitializeFieldName = "s_initialized";
        private const string InjectableFieldName = "s_injectableConstructorsCache";

        /// <summary>
        /// Resets static cache fields of TypeExtensions to force reloading of AppDomain assemblies/types.
        /// </summary>
        protected static void ResetDomainTypeCache()
        {
            var typeExt = typeof(TypeAnalyzes);

            var cachedField = typeExt.GetField(CacheDomainTypesFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            //var assembliesField = typeExt.GetField(AssemblyCacheFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            var initializedField = typeExt.GetField(InitializeFieldName, BindingFlags.Static | BindingFlags.NonPublic);

            cachedField?.SetValue(null, null);
            //assembliesField?.SetValue(null, null);
            initializedField?.SetValue(null, false);
        }

        protected static void ResetAssembliesCache()
        {
            var typeExt = typeof(TypeAnalyzes);

            MethodInfo? clear = typeExt.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic, Type.EmptyTypes);

            clear?.Invoke(null, null);
        }

        protected static HashSet<Assembly> GetAssembliesCache()
        {
            var typeExt = typeof(TypeAnalyzes);
            var field = typeExt.GetField(AssemblyCacheFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            return field?.GetValue(null) as HashSet<Assembly> ?? new HashSet<Assembly>();
        }

        protected static List<Type> GetCachedDomainTypes()
        {
            var typeExt = typeof(TypeAnalyzes);
            var field = typeExt.GetField(CacheDomainTypesFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            return field?.GetValue(null) as List<Type> ?? new List<Type>();
        }

        protected static bool IsInjectableConstructorsCacheEmpty()
        {
            var typeExt = typeof(TypeAnalyzes);
            var field = typeExt.GetField(InjectableFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            var cache = field?.GetValue(null) as ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConstructorInfo>>;
            return cache == null || cache.IsEmpty;
        }

        protected static bool IsDomainTypesCacheNull()
        {
            var typeExt = typeof(TypeAnalyzes);
            var field = typeExt.GetField(CacheDomainTypesFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            return field?.GetValue(null) == null;
        }

        protected static bool IsInitializedFlagFalse()
        {
            var typeExt = typeof(TypeAnalyzes);
            var field = typeExt.GetField(InitializeFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            return field != null && (bool)field.GetValue(null) == false;
        }
    }
}
