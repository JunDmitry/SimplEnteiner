using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class ClearCacheTests : TypeExtensionsTestBase
    {
        [Fact]
        public void ClearCache_Clears_InjectableConstructorsCache()
        {
            // Populate cache
            ResetDomainTypeCache();
            var type = typeof(OnePublicCtorWithAttribute);
            var ctor = type.GetInjectableConstructor(typeof(InjectAttribute)); // This populates cache

            // Verify cache is not empty
            Assert.False(IsInjectableConstructorsCacheEmpty());

            // Clear cache
            TypeAnalyzes.ClearCache();

            // Verify cache is empty
            Assert.True(IsInjectableConstructorsCacheEmpty());
        }

        [Fact]
        public void ClearCache_Clears_DomainTypesCache()
        {
            // Populate cache
            ResetDomainTypeCache();
            var type = typeof(ISimpleService);
            _ = type.FindAllAssignableFrom(); // This populates cache

            // Verify cache is not empty
            Assert.False(IsDomainTypesCacheNull());

            // Clear cache
            TypeAnalyzes.ClearCache();

            // Verify cache is cleared
            Assert.True(IsDomainTypesCacheNull());
        }

        [Fact]
        public void ClearCache_Sets_Initialized_Flag_To_False()
        {
            // Populate cache
            ResetDomainTypeCache();
            var type = typeof(ISimpleService);
            _ = type.FindAllAssignableFrom(); // This populates cache

            // Clear cache
            TypeAnalyzes.ClearCache();

            // Verify flag is false
            Assert.True(IsInitializedFlagFalse());
        }

        [Fact]
        public void ClearCache_Allows_Repopulation_Of_Cache()
        {
            // Populate cache
            ResetDomainTypeCache();
            var type = typeof(ISimpleService);
            _ = type.FindAllAssignableFrom(); // This populates cache

            // Clear cache
            TypeAnalyzes.ClearCache();

            // Repopulate cache
            _ = type.FindAllAssignableFrom();

            // Verify cache is populated again
            Assert.False(IsDomainTypesCacheNull());
            Assert.False(IsInitializedFlagFalse());
        }
    }
}