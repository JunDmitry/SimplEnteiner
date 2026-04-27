using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with parameter of array type.
    /// </summary>
    internal class ArrayDependencyClass
    {
        public ArrayDependencyClass(ISimpleService[] services) { }
    }

    /// <summary>
    /// Class with parameter of IEnumerable type.
    /// </summary>
    internal class EnumerableDependencyClass
    {
        public EnumerableDependencyClass(IEnumerable<ISimpleService> services) { }
    }

    /// <summary>
    /// Class with parameter of Lazy type.
    /// </summary>
    internal class LazyDependencyClass
    {
        public LazyDependencyClass(Lazy<ISimpleService> lazyService) { }
    }

    /// <summary>
    /// Class with parameter of Func type.
    /// </summary>
    internal class FuncDependencyClass
    {
        public FuncDependencyClass(Func<ISimpleService> factory) { }
    }

    /// <summary>
    /// Class with parameter of regular type.
    /// </summary>
    internal class RegularDependencyClass
    {
        public RegularDependencyClass(ISimpleService service) { }
    }
}
