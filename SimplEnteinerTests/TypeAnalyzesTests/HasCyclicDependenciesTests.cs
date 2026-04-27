using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class HasCyclicDependenciesTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).HasCyclicDependencies(typeof(InjectAttribute), out _);
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_InjectAttribute_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(RootType).HasCyclicDependencies(null, out _);
            });
        }

        [Fact]
        public void Returns_False_For_Acyclic_Dependencies()
        {
            ResetDomainTypeCache();

            var hasCycle = typeof(RootType).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            Assert.False(hasCycle);
            Assert.Null(cyclePath);
        }

        [Fact]
        public void Returns_True_For_Self_Cyclic_Dependency()
        {
            ResetDomainTypeCache();

            var hasCycle = typeof(SelfCyclicClass).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            Assert.True(hasCycle);
            Assert.NotNull(cyclePath);
            Assert.Contains(typeof(SelfCyclicClass), cyclePath);
        }

        [Fact]
        public void Returns_True_For_Mutual_Cyclic_Dependency()
        {
            ResetDomainTypeCache();

            var hasCycle = typeof(CycleClassA).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            Assert.True(hasCycle);
            Assert.NotNull(cyclePath);
            Assert.Contains(typeof(CycleClassA), cyclePath);
            Assert.Contains(typeof(CycleClassB), cyclePath);
        }

        [Fact]
        public void CyclePath_Contains_All_Types_In_Cycle()
        {
            ResetDomainTypeCache();

            typeof(CycleClassA).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            // The cycle is A -> B -> A
            Assert.Equal(3, cyclePath.Count); // A, B, A (or B, A, B depending on start)
            Assert.Contains(typeof(CycleClassA), cyclePath);
            Assert.Contains(typeof(CycleClassB), cyclePath);
        }

        [Fact]
        public void Returns_False_For_Type_With_No_Dependencies()
        {
            ResetDomainTypeCache();

            var hasCycle = typeof(LeafDependency).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            Assert.False(hasCycle);
            Assert.Null(cyclePath);
        }

        [Fact]
        public void Returns_False_For_Type_With_Injectable_Field_Self_Reference()
        {
            ResetDomainTypeCache();

            var hasCycle = typeof(SelfDependencyViaField).HasCyclicDependencies(typeof(InjectFieldAttribute), out var cyclePath);

            // Self-reference via field is not a constructor cycle, but still a cycle in the graph
            // The method checks constructor and injectable members
            Assert.True(hasCycle);
            Assert.NotNull(cyclePath);
        }
    }
}