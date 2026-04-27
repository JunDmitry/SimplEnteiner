using System.Reflection;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetDependencyTypeTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_ParameterInfo_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ParameterInfo)null).GetDependencyType();
            });
        }

        [Fact]
        public void Returns_Underlying_Type_For_Array()
        {
            ResetDomainTypeCache();

            var ctor = typeof(ArrayDependencyClass).GetConstructors().First();
            var parameter = ctor.GetParameters().First();

            var result = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), result);
        }

        [Fact]
        public void Returns_Underlying_Type_For_IEnumerable()
        {
            ResetDomainTypeCache();

            var ctor = typeof(EnumerableDependencyClass).GetConstructors().First();
            var parameter = ctor.GetParameters().First();

            var result = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), result);
        }

        [Fact]
        public void Returns_Underlying_Type_For_Lazy()
        {
            ResetDomainTypeCache();

            var ctor = typeof(LazyDependencyClass).GetConstructors().First();
            var parameter = ctor.GetParameters().First();

            var result = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), result);
        }

        [Fact]
        public void Returns_Underlying_Type_For_Func()
        {
            ResetDomainTypeCache();

            var ctor = typeof(FuncDependencyClass).GetConstructors().First();
            var parameter = ctor.GetParameters().First();

            var result = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), result);
        }

        [Fact]
        public void Returns_Same_Type_For_Regular_Dependency()
        {
            ResetDomainTypeCache();

            var ctor = typeof(RegularDependencyClass).GetConstructors().First();
            var parameter = ctor.GetParameters().First();

            var result = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), result);
        }

        [Fact]
        public void Returns_Underlying_Type_For_Complex_Parameters()
        {
            ResetDomainTypeCache();

            var ctor = typeof(ComplexParamCtorFactoryClass).GetConstructors().First();
            var parameters = ctor.GetParameters();

            // First parameter: IEnumerable<ISimpleService>
            var result1 = parameters[0].GetDependencyType();
            Assert.Equal(typeof(ISimpleService), result1);

            // Second parameter: Lazy<IGenericService<int>>
            var result2 = parameters[1].GetDependencyType();
            Assert.Equal(typeof(IGenericService<int>), result2);

            // Third parameter: Func<ISimpleService>
            var result3 = parameters[2].GetDependencyType();
            Assert.Equal(typeof(ISimpleService), result3);
        }
    }
}