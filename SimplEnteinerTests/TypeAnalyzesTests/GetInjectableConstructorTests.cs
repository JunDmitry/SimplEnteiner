using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetInjectableConstructorTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).GetInjectableConstructor(typeof(InjectAttribute));
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_InjectAttributeType_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(OnePublicCtorNoAttribute).GetInjectableConstructor(null);
            });
        }

        [Fact]
        public void Returns_Null_When_No_Public_Constructors()
        {
            ResetDomainTypeCache();

            var result = typeof(NoPublicCtorClass).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.Null(result);
        }

        [Fact]
        public void Returns_Null_When_No_Public_Constructors_OnlyInternal()
        {
            ResetDomainTypeCache();

            var result = typeof(OnlyInternalCtorClass).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.Null(result);
        }

        [Fact]
        public void Returns_Ctor_With_Attribute_When_One_Ctor_With_Attribute()
        {
            ResetDomainTypeCache();

            var result = typeof(OnePublicCtorWithAttribute).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.NotNull(result);
            Assert.True(result.IsDefined(typeof(InjectAttribute), true));
        }

        [Fact]
        public void Returns_Greediest_Ctor_When_No_Attribute()
        {
            ResetDomainTypeCache();

            var result = typeof(MultipleCtorNoAttribute).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.NotNull(result);
            Assert.Equal(2, result.GetParameters().Length);
        }

        [Fact]
        public void Throws_When_Multiple_Ctors_With_Attribute()
        {
            ResetDomainTypeCache();

            Assert.Throws<Exception>(() =>
            {
                typeof(MultipleCtorWithMultipleAttributes).GetInjectableConstructor(typeof(InjectAttribute));
            });
        }

        [Fact]
        public void Returns_Ctor_With_Specific_Attribute()
        {
            ResetDomainTypeCache();

            var result = typeof(MultipleCtorWithSecondAttribute).GetInjectableConstructor(typeof(SecondInjectAttribute));

            Assert.NotNull(result);
            Assert.True(result.IsDefined(typeof(SecondInjectAttribute), true));
        }

        [Fact]
        public void Caching_Works_Correctly()
        {
            ResetDomainTypeCache();

            var result1 = typeof(OnePublicCtorWithAttribute).GetInjectableConstructor(typeof(InjectAttribute));
            var result2 = typeof(OnePublicCtorWithAttribute).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.Same(result1, result2);
        }

        [Fact]
        public void Returns_Ctor_From_Base_Class()
        {
            ResetDomainTypeCache();

            var result = typeof(DerivedCtorClass).GetInjectableConstructor(typeof(InjectAttribute));

            // Should return the only public constructor (no attribute)
            Assert.NotNull(result);
        }

        [Fact]
        public void Returns_Marked_Ctor_From_Derived_When_Exists()
        {
            ResetDomainTypeCache();

            var result = typeof(DerivedWithExtraCtor).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.NotNull(result);
            Assert.True(result.IsDefined(typeof(InjectAttribute), true));
        }

        [Fact]
        public void Returns_Ctor_From_Generic_Class()
        {
            ResetDomainTypeCache();

            var result = typeof(GenericCtorClass<>).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.NotNull(result);
            Assert.True(result.IsDefined(typeof(InjectAttribute), true));
        }

        [Fact]
        public void Returns_Ctor_From_Closed_Generic_Class()
        {
            ResetDomainTypeCache();

            var result = typeof(ClosedGenericCtorClass).GetInjectableConstructor(typeof(InjectAttribute));

            Assert.NotNull(result);
            Assert.True(result.IsDefined(typeof(InjectAttribute), true));
        }
    }
}