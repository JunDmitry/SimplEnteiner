namespace SimplEnteinerTests.TestTypes.AtributeTypes
{
    /// <summary>
    /// Simple test attribute without parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    internal class SimpleTestAttribute : Attribute
    { }

    /// <summary>
    /// Test attribute with string parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    internal class StringParameterAttribute : Attribute
    {
        public StringParameterAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    /// <summary>
    /// Test attribute that can be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    internal class InheritableTestAttribute : Attribute
    { }

    /// <summary>
    /// Test attribute that cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    internal class NonInheritableTestAttribute : Attribute
    { }
}
