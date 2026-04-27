using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetAssignableToGenericArgumentsTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));
            });
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Class_Implementing_Generic_Interface()
        {
            ResetDomainTypeCache();

            var args = typeof(GenericServiceInt)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Class_With_String_Generic_Argument()
        {
            ResetDomainTypeCache();

            var args = typeof(GenericServiceString)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(string), args[0]);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Dual_Generic_Interface()
        {
            ResetDomainTypeCache();

            var args = typeof(DualGenericServiceIntString)
                .GetAssignableToGenericArguments(typeof(IDualGenericServiceDefinition<,>));

            Assert.Equal(2, args.Length);
            Assert.Equal(typeof(int), args[0]);
            Assert.Equal(typeof(string), args[1]);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Open_Generic_Class()
        {
            ResetDomainTypeCache();

            var args = typeof(OpenGenericServiceDefinition<>)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.True(args[0].IsGenericParameter);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Class_Implementing_Via_Base_Class()
        {
            ResetDomainTypeCache();

            var args = typeof(ServiceViaBaseClass)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Closed_Generic_Class_That_Is_Not_Open_Definition()
        {
            ResetDomainTypeCache();

            var args = typeof(ClosedFromOpenGenericService)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);
        }

        [Fact]
        public void Returns_Empty_For_Class_Implementing_Non_Generic_Interface()
        {
            ResetDomainTypeCache();

            var args = typeof(NonGenericServiceImplementation)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Empty(args);
        }

        [Fact]
        public void Returns_Empty_For_Class_Without_Required_Interface()
        {
            ResetDomainTypeCache();

            var args = typeof(UnrelatedService)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Empty(args);
        }

        [Fact]
        public void Returns_First_Matching_Interface_Generics_For_Class_With_Multiple_Generic_Interfaces()
        {
            ResetDomainTypeCache();

            var args = typeof(MultipleGenericService)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);
        }

        [Fact]
        public void Returns_Empty_For_Null_Generic_Definition()
        {
            ResetDomainTypeCache();

            // The method doesn't throw on null generic definition, but returns empty
            var args = typeof(GenericServiceInt)
                .GetAssignableToGenericArguments(null);

            // According to implementation, null will cause IsEqualOpenDefinition to fail (null != genericType)
            Assert.Empty(args);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_Nested_Generic_Class()
        {
            ResetDomainTypeCache();

            var nestedGeneric = typeof(NestedTypeVisibilityContainer.ClosedNestedIsAssignableGenericClass<>);
            var args = nestedGeneric
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.True(args[0].IsGenericParameter);
        }

        [Fact]
        public void Returns_Generic_Arguments_For_GenericClass_With_Constraints()
        {
            ResetDomainTypeCache();

            var args = typeof(ReferenceTypeConstrainedIsAssignableGenericService<string>)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.Equal(typeof(string), args[0]);
        }

        [Fact]
        public void Returns_Generic_Arguments_From_Base_Class_When_Interface_Implemented_In_Base()
        {
            ResetDomainTypeCache();

            var args = typeof(OpenGenericServiceViaBase<>)
                .GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.Single(args);
            Assert.True(args[0].IsGenericParameter);
        }
    }
}