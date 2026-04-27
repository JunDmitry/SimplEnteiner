using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes
{
    /// <summary>
    /// Simple parameterless delegate.
    /// </summary>
    internal delegate void SimpleDelegate();

    /// <summary>
    /// Generic delegate returning the same value it receives.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    /// <param name="value">Input value.</param>
    /// <returns>Output value.</returns>
    internal delegate T GenericDelegate<T>(T value);

    /// <summary>
    /// Delegate representing a factory of <see cref="ISimpleService"/>.
    /// </summary>
    /// <returns>Created service.</returns>
    internal delegate ISimpleService SimpleServiceFactory();

    /// <summary>
    /// Delegate representing a factory of <see cref="IGenericService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    /// <returns>Created service.</returns>
    internal delegate IGenericService<T> GenericServiceFactory<T>();

    /// <summary>
    /// Delegate for domain-type presence tests.
    /// </summary>
    internal delegate void TestDelegate();
}
