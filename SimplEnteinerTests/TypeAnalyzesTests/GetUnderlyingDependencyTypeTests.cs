using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetUnderlyingDependencyTypeTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).GetUnderlyingDependencyType();
            });
        }

        [Fact]
        public void Returns_Same_Type_For_Non_Wrapped_Types()
        {
            Assert.Equal(typeof(ISimpleService), typeof(ISimpleService).GetUnderlyingDependencyType());
            Assert.Equal(typeof(int), typeof(int).GetUnderlyingDependencyType());
            Assert.Equal(typeof(string), typeof(string).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Returns_ElementType_For_Array()
        {
            Assert.Equal(typeof(ISimpleService), typeof(ISimpleService[]).GetUnderlyingDependencyType());
            Assert.Equal(typeof(int), typeof(int[]).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Returns_Generic_Argument_For_IEnumerable()
        {
            Assert.Equal(typeof(ISimpleService), typeof(IEnumerable<ISimpleService>).GetUnderlyingDependencyType());
            Assert.Equal(typeof(int), typeof(IEnumerable<int>).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Returns_Generic_Argument_For_Lazy()
        {
            Assert.Equal(typeof(ISimpleService), typeof(Lazy<ISimpleService>).GetUnderlyingDependencyType());
            Assert.Equal(typeof(int), typeof(Lazy<int>).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Returns_Generic_Argument_For_Func()
        {
            Assert.Equal(typeof(ISimpleService), typeof(Func<ISimpleService>).GetUnderlyingDependencyType());
            Assert.Equal(typeof(int), typeof(Func<int>).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Does_Not_Unwrap_Other_Generic_Types()
        {
            Assert.Equal(typeof(IGenericService<int>), typeof(IGenericService<int>).GetUnderlyingDependencyType());
            Assert.Equal(typeof(List<int>), typeof(List<int>).GetUnderlyingDependencyType());
        }

        [Fact]
        public void Handles_Nested_Wrappers()
        {
            // IEnumerable<Func<int>> should return Func<int> (first level unwrap)
            Assert.Equal(typeof(Func<int>), typeof(IEnumerable<Func<int>>).GetUnderlyingDependencyType());
        }
    }
}