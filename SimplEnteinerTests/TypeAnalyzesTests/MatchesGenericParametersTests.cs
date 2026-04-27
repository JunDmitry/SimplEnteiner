using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class MatchesGenericParametersTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Args_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type[])null).MatchesGenericParameters(new Type[] { typeof(object) });
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_Constraints_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Type[] { typeof(object) }.MatchesGenericParameters(null);
            });
        }

        [Fact]
        public void Returns_True_When_Args_And_Constraints_Are_Empty()
        {
            Assert.True(new Type[] { }.MatchesGenericParameters(new Type[] { }));
        }

        [Fact]
        public void Returns_False_When_Args_And_Constraints_Lengths_Differ()
        {
            Assert.False(new Type[] { typeof(int) }.MatchesGenericParameters(new Type[] { typeof(object), typeof(object) }));
        }

        [Fact]
        public void Returns_True_When_Args_Match_Constraints_Exactly()
        {
            Assert.True(new Type[] { typeof(int) }.MatchesGenericParameters(new Type[] { typeof(int) }));
        }

        [Fact]
        public void Returns_True_When_Args_Are_Subtypes_Of_Constraints()
        {
            Assert.True(new Type[] { typeof(string) }.MatchesGenericParameters(new Type[] { typeof(object) }));
        }

        [Fact]
        public void Returns_False_When_Args_Are_Not_Subtypes_Of_Constraints()
        {
            Assert.False(new Type[] { typeof(int) }.MatchesGenericParameters(new Type[] { typeof(string) }));
        }

        [Fact]
        public void Returns_True_For_Multiple_Generic_Arguments()
        {
            Assert.True(new Type[] { typeof(int), typeof(string) }.MatchesGenericParameters(new Type[] { typeof(object), typeof(object) }));
        }

        [Fact]
        public void Returns_False_For_Multiple_Generic_Arguments_With_One_Mismatch()
        {
            Assert.False(new Type[] { typeof(int), typeof(string) }.MatchesGenericParameters(new Type[] { typeof(object), typeof(int) }));
        }

        [Fact]
        public void Returns_True_For_Interface_Constraints()
        {
            Assert.True(new Type[] { typeof(ConstraintImplementation) }.MatchesGenericParameters(new Type[] { typeof(IConstraintTest) }));
        }

        [Fact]
        public void Returns_False_For_Non_Implementing_Type()
        {
            Assert.False(new Type[] { typeof(NonConstraintImplementation) }.MatchesGenericParameters(new Type[] { typeof(IConstraintTest) }));
        }

        [Fact]
        public void Returns_True_For_ValueType_Constraint()
        {
            Assert.True(new Type[] { typeof(int) }.MatchesGenericParameters(new Type[] { typeof(ValueType) }));
        }

        [Fact]
        public void Returns_False_For_ReferenceType_When_ValueType_Expected()
        {
            Assert.False(new Type[] { typeof(string) }.MatchesGenericParameters(new Type[] { typeof(ValueType) }));
        }
    }
}