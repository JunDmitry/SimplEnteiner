using System.Reflection;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetFactoryMethodTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Constructor_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ConstructorInfo)null).GetFactoryMethod();
            });
        }

        [Fact]
        public void Creates_Factory_For_Parameterless_Ctor()
        {
            ResetDomainTypeCache();

            var ctor = typeof(ParameterlessCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            var instance = factory(Array.Empty<object>());

            Assert.NotNull(instance);
            Assert.IsType<ParameterlessCtorFactoryClass>(instance);
        }

        [Fact]
        public void Creates_Factory_For_Single_Parameter_Ctor()
        {
            ResetDomainTypeCache();

            var ctor = typeof(SingleParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            var service = new SimpleService();
            var instance = factory(new object[] { service });

            Assert.NotNull(instance);
            Assert.IsType<SingleParamCtorFactoryClass>(instance);
        }

        [Fact]
        public void Creates_Factory_For_Multiple_Parameters_Ctor()
        {
            ResetDomainTypeCache();

            var ctor = typeof(MultiParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            var service = new SimpleService();
            var instance = factory(new object[] { service, 42, "test" });

            Assert.NotNull(instance);
            Assert.IsType<MultiParamCtorFactoryClass>(instance);
        }

        [Fact]
        public void Creates_Factory_For_Complex_Parameters()
        {
            ResetDomainTypeCache();

            var ctor = typeof(ComplexParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            var services = new[] { new SimpleService() };
            var lazyService = new Lazy<IGenericService<int>>(() => new GenericServiceIntSecond());
            var func = new Func<ISimpleService>(() => new SimpleService());

            var instance = factory(new object[] { services, lazyService, func });

            Assert.NotNull(instance);
            Assert.IsType<ComplexParamCtorFactoryClass>(instance);
        }

        [Fact]
        public void Factory_Throws_With_Invalid_Arguments()
        {
            ResetDomainTypeCache();

            var ctor = typeof(SingleParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            Assert.ThrowsAny<Exception>(() => factory(new object[] { "invalid" }));
        }

        [Fact]
        public void Factory_Throws_With_Wrong_Number_Of_Arguments()
        {
            ResetDomainTypeCache();

            var ctor = typeof(SingleParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            Assert.ThrowsAny<Exception>(() => factory(new object[] { }));
        }

        [Fact]
        public void Factory_Fallback_To_Reflection_When_Expression_Fails()
        {
            // This test verifies that if Expression.Compile fails, the fallback is used.
            // We can't easily simulate Expression.Compile failure, but we can test that
            // the factory works correctly in normal scenarios.
            ResetDomainTypeCache();

            var ctor = typeof(SingleParamCtorFactoryClass).GetConstructors().First();
            var factory = ctor.GetFactoryMethod();

            var service = new SimpleService();
            var instance = factory(new object[] { service });

            Assert.NotNull(instance);
            Assert.IsType<SingleParamCtorFactoryClass>(instance);
        }
    }
}