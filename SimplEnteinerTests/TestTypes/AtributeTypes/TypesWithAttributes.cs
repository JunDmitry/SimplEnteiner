namespace SimplEnteinerTests.TestTypes.AtributeTypes
{
    /// <summary>
    /// Class with simple test attribute.
    /// </summary>
    [SimpleTest]
    internal class ClassWithSimpleAttribute
    { }

    /// <summary>
    /// Class with string parameter attribute.
    /// </summary>
    [StringParameter("test-value")]
    internal class ClassWithStringParameterAttribute
    { }

    /// <summary>
    /// Interface with simple test attribute.
    /// </summary>
    [SimpleTest]
    internal interface IInterfaceWithSimpleAttribute
    { }

    /// <summary>
    /// Class with inheritable attribute.
    /// </summary>
    [InheritableTest]
    internal class BaseClassWithInheritableAttribute
    { }

    /// <summary>
    /// Class inheriting from base with inheritable attribute (should be found with isInherit=true).
    /// </summary>
    internal class DerivedClassWithInheritableAttribute : BaseClassWithInheritableAttribute
    { }

    /// <summary>
    /// Class inheriting from base with non-inheritable attribute (should NOT be found with isInherit=true).
    /// </summary>
    internal class DerivedClassWithNonInheritableAttribute : BaseClassWithNonInheritableAttribute
    { }

    /// <summary>
    /// Base class with non-inheritable attribute.
    /// </summary>
    [NonInheritableTest]
    internal class BaseClassWithNonInheritableAttribute
    { }

    /// <summary>
    /// Class with multiple attributes.
    /// </summary>
    [SimpleTest]
    [StringParameter("multi")]
    internal class ClassWithMultipleAttributes
    { }

    /// <summary>
    /// Nested class with attribute.
    /// </summary>
    internal class ContainerWithNestedAttribute
    {
        [SimpleTest]
        public class NestedClassWithAttribute
        { }
    }

    /// <summary>
    /// Class without attributes.
    /// </summary>
    internal class ClassWithoutAttributes
    { }
}
