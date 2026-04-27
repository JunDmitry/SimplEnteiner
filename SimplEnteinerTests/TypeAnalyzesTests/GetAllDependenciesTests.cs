using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetAllDependenciesTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).GetAllDependencies(typeof(InjectAttribute));
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_InjectAttribute_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(RootType).GetAllDependencies(null);
            });
        }

        [Fact]
        public void Returns_Empty_For_Type_With_No_Dependencies()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(LeafDependency).GetAllDependencies(typeof(InjectAttribute));

            Assert.Empty(dependencies);
        }

        [Fact]
        public void Returns_Direct_Constructor_Dependencies()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(MiddleDependency).GetAllDependencies(typeof(InjectAttribute)).ToList();

            Assert.Single(dependencies);
            Assert.Contains(typeof(LeafDependency), dependencies);
        }

        [Fact]
        public void Returns_Transitive_Dependencies()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(RootType).GetAllDependencies(typeof(InjectAttribute)).ToList();

            Assert.Equal(2, dependencies.Count);
            Assert.Contains(typeof(MiddleDependency), dependencies);
            Assert.Contains(typeof(LeafDependency), dependencies);
        }

        [Fact]
        public void Returns_All_Dependencies_From_Multiple_Constructor_Parameters()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(MultipleDependencies).GetAllDependencies(typeof(InjectAttribute)).ToList();

            Assert.Equal(2, dependencies.Count);
            Assert.Contains(typeof(LeafDependency), dependencies);
            Assert.Contains(typeof(MiddleDependency), dependencies);
        }

        [Fact]
        public void Returns_Dependencies_From_Injectable_Field()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(FieldDependency).GetAllDependencies(typeof(InjectFieldAttribute)).ToList();

            Assert.Single(dependencies);
            Assert.Contains(typeof(LeafDependency), dependencies);
        }

        [Fact]
        public void Returns_Dependencies_From_Injectable_Property()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(PropertyDependency).GetAllDependencies(typeof(InjectPropertyAttribute)).ToList();

            Assert.Single(dependencies);
            Assert.Contains(typeof(LeafDependency), dependencies);
        }

        [Fact]
        public void Returns_Dependencies_From_Injectable_Method()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(MethodDependency).GetAllDependencies(typeof(InjectMethodAttribute)).ToList();

            Assert.Single(dependencies);
            Assert.Contains(typeof(LeafDependency), dependencies);
        }

        [Fact]
        public void Does_Not_Include_Self_In_Dependencies()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(SelfDependencyViaField).GetAllDependencies(typeof(InjectAttribute)).ToList();

            // Should not include SelfDependencyViaField itself
            Assert.DoesNotContain(typeof(SelfDependencyViaField), dependencies);
        }

        [Fact]
        public void Handles_Cyclic_Dependencies_Without_Infinite_Loop()
        {
            ResetDomainTypeCache();

            // This should not hang or throw
            var dependencies = typeof(CycleClassA).GetAllDependencies(typeof(InjectAttribute)).ToList();

            // Should include B but not A (since A is the root)
            Assert.Contains(typeof(CycleClassB), dependencies);
            Assert.DoesNotContain(typeof(CycleClassA), dependencies);
        }

        [Fact]
        public void Uses_Resolver_When_Provided()
        {
            ResetDomainTypeCache();

            // Resolver that maps LeafDependency to a different type
            Func<Type, Type> resolver = t => t == typeof(LeafDependency) ? typeof(MiddleDependency) : t;

            var dependencies = typeof(LeafDependency).GetAllDependencies(typeof(InjectAttribute), resolver).ToList();

            // With resolver, LeafDependency should map to MiddleDependency
            // But LeafDependency has no dependencies, so result should be empty
            Assert.Empty(dependencies);
        }

        [Fact]
        public void Uses_Resolver_For_Transitive_Dependencies()
        {
            ResetDomainTypeCache();

            // Resolver that maps LeafDependency to a different type
            Func<Type, Type> resolver = t => t == typeof(LeafDependency) ? typeof(RootType) : t;

            var dependencies = typeof(MiddleDependency).GetAllDependencies(typeof(InjectAttribute), resolver).ToList();

            // MiddleDependency depends on LeafDependency, which maps to MiddleDependency
            // So we get MiddleDependency as a dependency
            Assert.Single(dependencies);
            Assert.Contains(typeof(RootType), dependencies);
        }
    }
}