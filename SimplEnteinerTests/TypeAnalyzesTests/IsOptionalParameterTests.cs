using System.Reflection;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class IsOptionalParameterTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ParameterInfo)null).IsOptionalParameter();
            });
        }

        [Fact]
        public void Returns_True_For_Parameter_With_Optional_Attribute()
        {
            var ctor = typeof(ClassWithOptionalParameter).GetConstructors().First();
            var parameter = ctor.GetParameters()[0];

            Assert.True(parameter.IsOptionalParameter());
        }

        [Fact]
        public void Returns_True_For_Parameter_With_Default_Value()
        {
            var ctor = typeof(ClassWithDefaultParameter).GetConstructors().First();
            var parameter = ctor.GetParameters()[0];

            Assert.True(parameter.IsOptionalParameter());
        }

        [Fact]
        public void Returns_False_For_Required_Parameter()
        {
            var ctor = typeof(ClassWithoutOptionalParameters).GetConstructors().First();
            var parameter = ctor.GetParameters()[0];

            Assert.False(parameter.IsOptionalParameter());
        }

        [Fact]
        public void Returns_True_For_Mixed_Parameters()
        {
            var ctor = typeof(ClassWithMixedParameters).GetConstructors().First();
            var parameters = ctor.GetParameters();

            Assert.False(parameters[0].IsOptionalParameter()); // required
            Assert.True(parameters[1].IsOptionalParameter());  // optional
        }
    }
}