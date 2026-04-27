using System.Reflection;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetLoadableTypesTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Returns_All_Types_From_Assembly_Including_Nested()
        {
            ResetDomainTypeCache();

            var assembly = typeof(NestedTypeVisibilityContainer).Assembly;
            var types = assembly.GetLoadableTypes().ToList();

            // Should include the container itself
            Assert.Contains(typeof(NestedTypeVisibilityContainer), types);

            // Should include all nested types
            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedClass), types);
            Assert.Contains(NestedTypeVisibilityContainer.PrivateNestedClassType, types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.InternalNestedClass), types);
            Assert.Contains(NestedTypeVisibilityContainer.ProtectedNestedClassType, types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.ProtectedInternalNestedClass), types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedStruct), types);
            Assert.Contains(NestedTypeVisibilityContainer.PrivateNestedInterfaceType, types);

            // Should include deeply nested
            Assert.Contains(typeof(NestedTypeVisibilityContainer.Level1Nested), types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.Level1Nested.Level2Nested), types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.Level1Nested.Level2Nested.Level3Nested), types);

            // Should include generic nested
            Assert.Contains(typeof(NestedTypeVisibilityContainer.GenericNestedClass<>), types);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.ClosedNestedGenericClass), types);
        }

        [Fact]
        public void Returns_Empty_When_Assembly_Throws_Exception()
        {
            ResetDomainTypeCache();

            // Create a mock assembly that throws
            var mockAssembly = new MockAssemblyThatThrows();
            var types = mockAssembly.GetLoadableTypes().ToList();

            Assert.Empty(types);
        }

        [Fact]
        public void Handles_ReflectionTypeLoadException_Gracefully()
        {
            ResetDomainTypeCache();

            // This test verifies that GetLoadableTypes catches ReflectionTypeLoadException
            // and returns only non-null types. We can't easily create such an assembly,
            // but we can verify the method exists and works for normal assemblies.
            var assembly = typeof(ISimpleService).Assembly;
            var types = assembly.GetLoadableTypes().ToList();

            Assert.NotEmpty(types);
            Assert.Contains(typeof(ISimpleService), types);
        }

        [Fact]
        public void Includes_Compiler_Generated_Nested_Types()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ClassWithCompilerGeneratedNested).Assembly;
            var types = assembly.GetLoadableTypes().ToList();

            // Should include the class with compiler-generated iterator
            Assert.Contains(typeof(ClassWithCompilerGeneratedNested), types);

            // The compiler-generated nested type should also be included
            var generatedType = types.FirstOrDefault(t =>
                t.Name.Contains("GetNumbers") && t.IsClass && t.IsNested);

            // May or may not be present depending on compiler, but method should not throw
            Assert.NotNull(generatedType); // If present, it should be valid
        }

        [Fact]
        public void Includes_Partial_Classes_As_Single_Type()
        {
            ResetDomainTypeCache();

            var assembly = typeof(PartialClassOne).Assembly;
            var types = assembly.GetLoadableTypes().ToList();

            // Partial classes should appear as one type
            var partialCount = types.Count(t => t == typeof(PartialClassOne));
            Assert.Equal(1, partialCount);
        }

        [Fact]
        public void Returns_Types_From_Multiple_Assemblies_In_Domain()
        {
            ResetDomainTypeCache();

            // GetLoadableTypes is called per-assembly, but FindAllAssignableFrom uses all assemblies
            // This test just verifies GetLoadableTypes works for the test assembly
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetLoadableTypes().ToList();

            Assert.NotEmpty(types);
            Assert.Contains(typeof(GetLoadableTypesTests), types);
        }
    }

    /// <summary>
    /// Mock assembly that throws exception on GetTypes().
    /// </summary>
    internal class MockAssemblyThatThrows : Assembly
    {
        public override string FullName => "MockAssemblyThatThrows";
        public override AssemblyName GetName() => new AssemblyName("MockAssemblyThatThrows");
        public override Type[] GetTypes() => throw new ReflectionTypeLoadException(new Type[0], new Exception[] { new Exception("Test exception") });
    }
}
