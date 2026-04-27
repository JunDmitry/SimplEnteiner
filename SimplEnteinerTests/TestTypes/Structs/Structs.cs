using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Structs
{
    /// <summary>
    /// Simple struct without fields.
    /// </summary>
    internal struct SimpleStruct
    { }

    /// <summary>
    /// Generic struct without fields.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal struct GenericStruct<T>
    { }

    /// <summary>
    /// Struct implementing a marker interface.
    /// </summary>
    internal struct StructImplementingMarker : IMarker
    { }

    /// <summary>
    /// Disposable struct.
    /// </summary>
    internal struct DisposableStruct : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Struct with a primitive field.
    /// </summary>
    internal struct StructWithValueField
    {
        public int Value;
    }

    /// <summary>
    /// Struct implementing <see cref="ITestAssignable"/>.
    /// </summary>
    internal struct TestAssignableStruct : ITestAssignable
    { }

    /// <summary>
    /// Generic struct implementing <see cref="ITestAssignable"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal struct TestAssignableGenericStruct<T> : ITestAssignable
    { }
}
