using SimplEnteinerTests.TestTypes.WithConstraints;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class SatisfiesOpenedGenericConstraintsTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).SatisfiesOpenedGenericConstraints(typeof(IGenericWithClassConstraint<>));
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_OpenDefinition_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(ClosedGenericWithClassConstraint).SatisfiesOpenedGenericConstraints(null);
            });
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Implementing_Open_Generic_With_Class_Constraint()
        {
            Assert.True(typeof(ClosedGenericWithClassConstraint)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithClassConstraint<>)));
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Implementing_Open_Generic_With_Struct_Constraint()
        {
            Assert.True(typeof(ClosedGenericWithStructConstraint)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithStructConstraint<>)));
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Implementing_Open_Generic_With_New_Constraint()
        {
            Assert.True(typeof(ClosedGenericWithNewConstraint)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithNewConstraint<>)));
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Implementing_Open_Generic_With_Interface_Constraint()
        {
            Assert.True(typeof(ClosedGenericWithInterfaceConstraint)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithInterfaceConstraint<>)));
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Implementing_Open_Generic_With_Multiple_Constraints()
        {
            Assert.True(typeof(ClosedGenericWithMultipleConstraints)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithMultipleConstraints<>)));
        }

        [Fact]
        public void Returns_True_For_Open_Generic_Implementing_Open_Generic_With_Satisfied_Constraints()
        {
            // OpenGenericWithClassConstraint<string> is not a type, but OpenGenericWithClassConstraint<T> is open
            // We check if the open generic itself satisfies constraints (it should, as T is constrained to class)
            Assert.True(typeof(OpenGenericWithClassConstraint<>)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithClassConstraint<>)));
        }

        [Fact]
        public void Returns_False_For_Type_Not_Implementing_Open_Generic()
        {
            Assert.False(typeof(ConstraintImplementation)
                .SatisfiesOpenedGenericConstraints(typeof(IGenericWithClassConstraint<>)));
        }
    }
}