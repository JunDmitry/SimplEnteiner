using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with parameterless constructor.
    /// </summary>
    internal class ParameterlessCtorFactoryClass
    {
        public ParameterlessCtorFactoryClass() { }
    }

    /// <summary>
    /// Class with single parameter constructor.
    /// </summary>
    internal class SingleParamCtorFactoryClass
    {
        public SingleParamCtorFactoryClass(ISimpleService service) { }
    }

    /// <summary>
    /// Class with multiple parameters constructor.
    /// </summary>
    internal class MultiParamCtorFactoryClass
    {
        public MultiParamCtorFactoryClass(ISimpleService service, int value, string name) { }
    }

    /// <summary>
    /// Class with complex parameter types.
    /// </summary>
    internal class ComplexParamCtorFactoryClass
    {
        public ComplexParamCtorFactoryClass(
            IEnumerable<ISimpleService> services,
            Lazy<IGenericService<int>> lazyService,
            Func<ISimpleService> factory)
        { }
    }
}
