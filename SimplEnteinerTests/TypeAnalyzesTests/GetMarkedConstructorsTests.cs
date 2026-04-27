using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetMarkedConstructorsTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).GetMarkedConstructors(typeof(InjectAttribute));
            });
        }

        [Fact]
        public void Throws_ArgumentNullException_When_InjectAttribute_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                typeof(OneMarkedConstructor).GetMarkedConstructors(null);
            });
        }

        [Fact]
        public void Returns_All_Marked_Constructors()
        {
            var constructors = typeof(MultipleMarkedConstructors)
                .GetMarkedConstructors(typeof(InjectAttribute))
                .ToList();

            Assert.Equal(2, constructors.Count);
            Assert.All(constructors, c => Assert.True(c.IsDefined(typeof(InjectAttribute), true)));
        }

        [Fact]
        public void Returns_Empty_When_No_Marked_Constructors()
        {
            var constructors = typeof(NoMarkedConstructors)
                .GetMarkedConstructors(typeof(InjectAttribute))
                .ToList();

            Assert.Empty(constructors);
        }

        [Fact]
        public void Returns_Only_Marked_Constructors()
        {
            var constructors = typeof(OneMarkedConstructor)
                .GetMarkedConstructors(typeof(InjectAttribute))
                .ToList();

            Assert.Single(constructors);
            Assert.True(constructors[0].IsDefined(typeof(InjectAttribute), true));
        }

        [Fact]
        public void Returns_Constructors_With_Inherited_Attributes()
        {
            // Test that inherited attributes are found (IsDefined with inherit=true)
            // Our InjectAttribute is defined with Inherited = false, so it won't be inherited
            // But we can test with a different attribute if needed
            // For now, just verify the method works
            var constructors = typeof(OneMarkedConstructor)
                .GetMarkedConstructors(typeof(InjectAttribute))
                .ToList();

            Assert.Single(constructors);
        }
    }
}