using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes
{
    /// <summary>
    /// Generic interface definition.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface IGenericServiceDefinition<T>
    { }

    /// <summary>
    /// Generic interface definition with two parameters.
    /// </summary>
    /// <typeparam name="T1">First generic argument.</typeparam>
    /// <typeparam name="T2">Second generic argument.</typeparam>
    internal interface IDualGenericServiceDefinition<T1, T2>
    { }

    /// <summary>
    /// Non-generic interface.
    /// </summary>
    internal interface INonGenericService
    { }

    /// <summary>
    /// Generic interface derived from another generic interface.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface IDerivedGenericService<T> : IGenericServiceDefinition<T>
    { }
}
