using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with injectable field.
    /// </summary>
    internal class ClassWithInjectableField
    {
        [InjectField]
        public ISimpleService Field;
    }

    /// <summary>
    /// Class with injectable property.
    /// </summary>
    internal class ClassWithInjectableProperty
    {
        [InjectProperty]
        public ISimpleService Property { get; set; }
    }

    /// <summary>
    /// Class with injectable method with multiple parameters.
    /// </summary>
    internal class ClassWithInjectableMethod
    {
        [InjectMethod]
        public void Method(ISimpleService service, int value) { }
    }

    /// <summary>
    /// Class with non-injectable member.
    /// </summary>
    internal class ClassWithNonInjectableMember
    {
        public ISimpleService Field;
    }
}
