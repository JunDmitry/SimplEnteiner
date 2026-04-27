using SimplEnteinerTests.TestTypes.WithConstraints;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class SatisfiesClosedGenericConstraintsTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).SatisfiesClosedGenericConstraints(typeof(ClosedGenericWithClassConstraint));
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_ClosedGenericDefinition_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(ClosedGenericWithClassConstraint).SatisfiesClosedGenericConstraints(null);
            });
        }

        [Fact]
        public void Throws_ArgumentException_When_ClosedGenericDefinition_Is_Not_Closed_Generic()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                typeof(ClosedGenericWithClassConstraint).SatisfiesClosedGenericConstraints(typeof(IGenericWithClassConstraint<>));
            });
        }

        [Fact]
        public void Throws_ArgumentException_When_ClosedGenericDefinition_Is_Open_Generic()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                typeof(ClosedGenericWithClassConstraint).SatisfiesClosedGenericConstraints(typeof(OpenGenericWithClassConstraint<>));
            });
        }

        [Fact]
        public void Returns_True_For_Type_Implementing_Closed_Generic_With_Satisfied_Constraints()
        {
            Assert.True(typeof(ClosedGenericWithClassConstraint)
                .SatisfiesClosedGenericConstraints(typeof(IGenericWithClassConstraint<string>)));
        }

        [Fact]
        public void Returns_False_For_Type_Not_Implementing_Closed_Generic()
        {
            Assert.False(typeof(ConstraintImplementation)
                .SatisfiesClosedGenericConstraints(typeof(IGenericWithClassConstraint<string>)));
        }

        [Fact]
        public void Returns_True_For_Derived_Type_Implementing_Closed_Generic()
        {
            // ClosedGenericWithClassConstraint implements IGenericWithClassConstraint<string>
            // We check if it satisfies the constraints of ClosedGenericWithClassConstraint (which is the same type)
            Assert.True(typeof(ClosedGenericWithClassConstraint)
                .SatisfiesClosedGenericConstraints(typeof(ClosedGenericWithClassConstraint)));
        }
    }
}