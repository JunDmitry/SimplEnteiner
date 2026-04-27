using System.Runtime.InteropServices;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with parameter marked as Optional.
    /// </summary>
    internal class ClassWithOptionalParameter
    {
        public ClassWithOptionalParameter([Optional] ISimpleService service) { }
    }

    /// <summary>
    /// Class with parameter with default value.
    /// </summary>
    internal class ClassWithDefaultParameter
    {
        public ClassWithDefaultParameter(ISimpleService service = null) { }
    }

    /// <summary>
    /// Class with multiple parameters, some optional.
    /// </summary>
    internal class ClassWithMixedParameters
    {
        public ClassWithMixedParameters(ISimpleService required, [Optional] int optional) { }
    }

    /// <summary>
    /// Class with no optional parameters.
    /// </summary>
    internal class ClassWithoutOptionalParameters
    {
        public ClassWithoutOptionalParameters(ISimpleService service) { }
    }
}
