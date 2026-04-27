using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class IsAssignableToGenericTypeDefinitionTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>));
            });
        }

        [Fact]
        public void Returns_True_For_Class_Implementing_Generic_Interface()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(GenericServiceInt)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));

            Assert.True(typeof(GenericServiceString)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_True_For_Class_Implementing_Dual_Generic_Interface()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(DualGenericServiceIntString)
                .IsAssignableToGenericTypeDefinition(typeof(IDualGenericServiceDefinition<,>)));
        }

        [Fact]
        public void Returns_True_For_Open_Generic_Class_Implementing_Generic_Interface()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(OpenGenericServiceDefinition<>)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_True_For_Class_Implementing_Via_Base_Class()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(ServiceViaBaseClass)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));

            Assert.True(typeof(OpenGenericServiceViaBase<>)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_True_For_Class_Implementing_Multiple_Generic_Interfaces()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(MultipleGenericService)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));

            Assert.True(typeof(MultipleGenericService)
                .IsAssignableToGenericTypeDefinition(typeof(IDualGenericServiceDefinition<,>)));
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Class_That_Is_Not_Open_Definition()
        {
            ResetDomainTypeCache();

            // ClosedFromOpenGenericService inherits from OpenGenericServiceDefinition<int>
            // It implements IGenericServiceDefinition<int> via base class
            Assert.True(typeof(ClosedFromOpenGenericService)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_False_For_Class_Implementing_Non_Generic_Interface()
        {
            ResetDomainTypeCache();

            Assert.False(typeof(NonGenericServiceImplementation)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_False_For_Class_Without_Required_Interface()
        {
            ResetDomainTypeCache();

            Assert.False(typeof(UnrelatedService)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_False_For_Interface_That_Is_Not_Generic_Definition()
        {
            ResetDomainTypeCache();

            // INonGenericService is not a generic definition
            Assert.False(typeof(INonGenericService)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_True_For_Class_With_Mixed_Interfaces()
        {
            ResetDomainTypeCache();

            Assert.True(typeof(MixedServiceImplementation)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_False_For_Null_Generic_Definition()
        {
            ResetDomainTypeCache();

            Assert.False(typeof(GenericServiceInt)
                .IsAssignableToGenericTypeDefinition(null));
        }

        [Fact]
        public void Returns_True_For_Nested_Generic_Class()
        {
            // Test nested generic class (if exists, otherwise create one)
            // For this test, we'll use existing nested types from previous tests
            ResetDomainTypeCache();

            // Use nested open generic from previous test types
            var nestedGeneric = typeof(NestedTypeVisibilityContainer.ClosedNestedIsAssignableGenericClass<>);
            Assert.True(nestedGeneric
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }

        [Fact]
        public void Returns_True_For_GenericClass_With_Constraints()
        {
            ResetDomainTypeCache();

            // Use constrained generic types from previous tests
            Assert.True(typeof(ReferenceTypeConstrainedIsAssignableGenericService<string>)
                .IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));
        }
    }
}