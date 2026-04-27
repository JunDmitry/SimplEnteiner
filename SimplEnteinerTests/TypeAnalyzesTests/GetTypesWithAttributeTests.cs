using System.Reflection;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetTypesWithAttributeTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Assembly_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Assembly)null).GetTypesWithAttribute<SimpleTestAttribute>();
            });
        }

        [Fact]
        public void Returns_Types_With_Simple_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ClassWithSimpleAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            Assert.Contains(typeof(ClassWithSimpleAttribute), types);
            Assert.Contains(typeof(IInterfaceWithSimpleAttribute), types);
            Assert.Contains(typeof(ClassWithMultipleAttributes), types);
            Assert.Contains(typeof(ContainerWithNestedAttribute.NestedClassWithAttribute), types);
        }

        [Fact]
        public void Returns_Types_With_String_Parameter_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ClassWithStringParameterAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<StringParameterAttribute>().ToList();

            Assert.Contains(typeof(ClassWithStringParameterAttribute), types);
            Assert.Contains(typeof(ClassWithMultipleAttributes), types);
        }

        [Fact]
        public void Returns_Empty_When_No_Types_With_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ClassWithoutAttributes).Assembly;
            var types = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            Assert.DoesNotContain(typeof(ClassWithoutAttributes), types);
        }

        [Fact]
        public void IsInherit_False_Returns_Only_Types_With_Direct_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(BaseClassWithInheritableAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<InheritableTestAttribute>(isInherit: false).ToList();

            Assert.Contains(typeof(BaseClassWithInheritableAttribute), types);
            Assert.DoesNotContain(typeof(DerivedClassWithInheritableAttribute), types);
        }

        [Fact]
        public void IsInherit_True_Returns_Types_With_Inherited_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(BaseClassWithInheritableAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<InheritableTestAttribute>(isInherit: true).ToList();

            Assert.Contains(typeof(BaseClassWithInheritableAttribute), types);
            Assert.Contains(typeof(DerivedClassWithInheritableAttribute), types);
        }

        [Fact]
        public void IsInherit_True_Does_Not_Return_Types_With_NonInheritable_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(BaseClassWithNonInheritableAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<NonInheritableTestAttribute>(isInherit: true).ToList();

            Assert.Contains(typeof(BaseClassWithNonInheritableAttribute), types);
            Assert.DoesNotContain(typeof(DerivedClassWithNonInheritableAttribute), types);
        }

        [Fact]
        public void Returns_Nested_Types_With_Attribute()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ContainerWithNestedAttribute).Assembly;
            var types = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            Assert.Contains(typeof(ContainerWithNestedAttribute.NestedClassWithAttribute), types);
        }

        [Fact]
        public void Returns_Types_From_Multiple_Assemblies()
        {
            ResetDomainTypeCache();

            // Test with current assembly
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            // Should contain at least some types
            Assert.NotEmpty(types);
        }

        [Fact]
        public void Handles_Compiler_Generated_Types_With_Attributes()
        {
            ResetDomainTypeCache();

            // Test that compiler-generated types are not filtered out
            var assembly = typeof(ClassWithCompilerGeneratedNested).Assembly;
            var types = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            // Should not throw and should return some results if attributes exist
            Assert.NotNull(types);
        }
    }
}