using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Open generic implementation constrained to reference types.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ReferenceTypeConstrainedGenericService<T> : IGenericService<T>
        where T : class
    { }

    /// <summary>
    /// Open generic implementation constrained to value types.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ValueTypeConstrainedGenericService<T> : IGenericService<T>
        where T : struct
    { }

    /// <summary>
    /// Open generic implementation constrained to types with a public parameterless constructor.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class DefaultCtorConstrainedGenericService<T> : IGenericService<T>
        where T : new()
    { }

    /// <summary>
    /// Open generic implementation constrained to <see cref="ISimpleService"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class InterfaceConstrainedGenericService<T> : IGenericService<T>
        where T : ISimpleService
    { }

    /// <summary>
    /// Open generic implementation with several constraints.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ComplexConstrainedGenericService<T> : IGenericService<T>
        where T : class, ISimpleService, new()
    { }

    /// <summary>
    /// Open generic implementation constrained to reference types.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ReferenceTypeConstrainedIsAssignableGenericService<T> : IGenericServiceDefinition<T>
        where T : class
    { }
}
