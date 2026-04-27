namespace SimplEnteinerTests.TestTypes.AtributeTypes
{
    /// <summary>
    /// Attribute to mark injectable constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    internal class InjectAttribute : Attribute
    { }

    /// <summary>
    /// Second attribute for testing multiple attributes scenario.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    internal class SecondInjectAttribute : Attribute
    { }

    /// <summary>
    /// Attribute to mark injectable field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    internal class InjectFieldAttribute : Attribute
    { }

    /// <summary>
    /// Attribute to mark injectable property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal class InjectPropertyAttribute : Attribute
    { }

    /// <summary>
    /// Attribute to mark injectable method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal class InjectMethodAttribute : Attribute
    { }
}
