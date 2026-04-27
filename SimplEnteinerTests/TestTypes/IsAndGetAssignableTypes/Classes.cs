using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes
{
    /// <summary>
    /// Class implementing <see cref="IGenericServiceDefinition{T}"/> with int.
    /// </summary>
    internal class GenericServiceInt : IGenericServiceDefinition<int>
    { }

    /// <summary>
    /// Class implementing <see cref="IGenericServiceDefinition{T}"/> with string.
    /// </summary>
    internal class GenericServiceString : IGenericServiceDefinition<string>
    { }

    /// <summary>
    /// Class implementing <see cref="IDualGenericServiceDefinition{T1, T2}"/> with int and string.
    /// </summary>
    internal class DualGenericServiceIntString : IDualGenericServiceDefinition<int, string>
    { }

    /// <summary>
    /// Open generic class implementing <see cref="IGenericServiceDefinition{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class OpenGenericServiceDefinition<T> : IGenericServiceDefinition<T>
    { }

    /// <summary>
    /// Closed generic class inheriting from open generic class.
    /// </summary>
    internal class ClosedFromOpenGenericService : OpenGenericServiceDefinition<int>
    { }

    /// <summary>
    /// Class implementing multiple generic interfaces.
    /// </summary>
    internal class MultipleGenericService :
        IGenericServiceDefinition<int>,
        IDualGenericServiceDefinition<string, double>
    { }

    /// <summary>
    /// Class implementing generic interface via base class.
    /// </summary>
    internal class ServiceViaBaseClass : GenericServiceInt
    { }

    /// <summary>
    /// Class implementing generic interface via base class (open generic).
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class OpenGenericServiceViaBase<T> : OpenGenericServiceDefinition<T>
    { }

    /// <summary>
    /// Class implementing non-generic interface.
    /// </summary>
    internal class NonGenericServiceImplementation : INonGenericService
    { }

    /// <summary>
    /// Class implementing both generic and non-generic interfaces.
    /// </summary>
    internal class MixedServiceImplementation : IGenericServiceDefinition<int>, INonGenericService
    { }

    /// <summary>
    /// Class that does not implement any of the test interfaces.
    /// </summary>
    internal class UnrelatedService
    { }

    internal class GenericServiceIntSecond : IGenericService<int> { }
}
