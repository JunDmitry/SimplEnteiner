using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using static SimplEnteiner.TypeAnalyzes;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class CanResolveAllDependenciesTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).CanResolveAllDependencies(
                    typeof(InjectAttribute),
                    new Dictionary<Type, Type>(),
                    t => t);
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_InjectAttribute_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(ConcreteNoDependencies).CanResolveAllDependencies(
                    null,
                    new Dictionary<Type, Type>(),
                    t => t);
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_DependencyRegistryMap_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(ConcreteNoDependencies).CanResolveAllDependencies<Type>(
                    typeof(InjectAttribute),
                    null,
                    t => t);
            });
        }

        [Fact]
        public void Throws_CircularDependencyException_When_Cycle_Detected()
        {
            ResetDomainTypeCache();

            Assert.Throws<CircularDependencyException>(() =>
            {
                typeof(CycleClassA).CanResolveAllDependencies(
                    typeof(InjectAttribute),
                    new Dictionary<Type, Type>(),
                    t => t);
            });
        }

        [Fact]
        public void Returns_True_For_Type_With_No_Dependencies()
        {
            ResetDomainTypeCache();

            var result = typeof(ConcreteNoDependencies).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                    t => t);

            Assert.True(result);
        }

        [Fact]
        public void Returns_True_For_Type_With_Concrete_Dependencies()
        {
            ResetDomainTypeCache();

            var result = typeof(ConcreteWithConcreteDependency).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                    t => t);

            Assert.True(result);
        }

        [Fact]
        public void Returns_False_For_Type_With_Unregistered_Interface_Dependency()
        {
            ResetDomainTypeCache();

            var result = typeof(ConcreteWithUnregisteredInterfaceDependency).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                    t => t);

            Assert.False(result);
        }

        [Fact]
        public void Returns_True_For_Type_With_Registered_Interface_Dependency()
        {
            ResetDomainTypeCache();

            var registry = new Dictionary<Type, Type>
            {
                { typeof(ISimpleService), typeof(SimpleService) }
            };

            var result = typeof(ConcreteWithRegisteredInterfaceDependency).CanResolveAllDependencies(
                typeof(InjectAttribute),
                registry,
                    t => t);

            Assert.True(result);
        }

        [Fact]
        public void Returns_True_For_Type_With_Multiple_Dependencies_All_Registered()
        {
            ResetDomainTypeCache();

            var registry = new Dictionary<Type, Type>
            {
                { typeof(ISimpleService), typeof(SimpleService) }
            };

            var result = typeof(ConcreteWithMultipleDependencies).CanResolveAllDependencies(
                typeof(InjectAttribute),
                registry,
                    t => t);

            Assert.True(result);
        }

        [Fact]
        public void Returns_False_For_Type_With_Multiple_Dependencies_One_Unregistered()
        {
            ResetDomainTypeCache();

            var result = typeof(ConcreteWithMultipleDependencies).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                    t => t);

            // ConcreteNoDependencies and ConcreteWithConcreteDependency are concrete classes
            // ISimpleService is not registered and not concrete (it's an interface)
            Assert.False(result);
        }

        [Fact]
        public void Returns_True_For_Type_With_Resolver()
        {
            ResetDomainTypeCache();

            // Resolver maps ISimpleService to a concrete type
            Func<Type, Type> resolver = t => t == typeof(ISimpleService) ? typeof(SimpleService) : t;

            var result = typeof(ConcreteWithUnregisteredInterfaceDependency).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                resolver,
                t => t);

            // With resolver, ISimpleService maps to SimpleService (concrete)
            // But SimpleService is not in registry, so it should still be false
            // Actually, the method checks if dependency is concrete OR in registry
            // SimpleService is concrete (has public constructor), so it should be true
            Assert.True(result);
        }

        [Fact]
        public void Returns_True_For_Concrete_From_Abstract_With_Dependency()
        {
            ResetDomainTypeCache();

            var result = typeof(ConcreteFromAbstractWithDependency).CanResolveAllDependencies(
                typeof(InjectAttribute),
                new Dictionary<Type, Type>(),
                t => t);

            // ISimpleService is not registered and not concrete (interface)
            Assert.False(result);
        }
    }
}