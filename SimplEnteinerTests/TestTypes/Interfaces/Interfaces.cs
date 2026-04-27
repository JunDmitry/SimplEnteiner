using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplEnteinerTests.TestTypes.Interfaces
{
    /// <summary>
    /// Simple marker interface for basic resolution tests.
    /// </summary>
    internal interface ISimpleService
    { }

    /// <summary>
    /// Interface derived from <see cref="ISimpleService"/>.
    /// </summary>
    internal interface IFirstDerivedSimpleService : ISimpleService
    { }

    /// <summary>
    /// Interface derived from <see cref="ISimpleService"/>.
    /// </summary>
    internal interface ISecondDerivedSimpleService : ISimpleService
    { }

    /// <summary>
    /// Simple marker interface.
    /// </summary>
    internal interface IMarker
    { }

    /// <summary>
    /// Second simple marker interface.
    /// </summary>
    internal interface IAnotherMarker
    { }

    /// <summary>
    /// Interface that inherits from two marker interfaces.
    /// </summary>
    internal interface IMultiInterfaceService : IMarker, IAnotherMarker
    { }

    /// <summary>
    /// Simple generic service interface.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface IGenericService<T>
    { }

    /// <summary>
    /// Generic interface derived from <see cref="IGenericService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface IGenericDerivedService<T> : IGenericService<T>
    { }

    /// <summary>
    /// Generic interface with two type arguments.
    /// </summary>
    /// <typeparam name="TLeft">Left generic argument.</typeparam>
    /// <typeparam name="TRight">Right generic argument.</typeparam>
    internal interface IDualGenericService<TLeft, TRight>
    { }

    /// <summary>
    /// Covariant generic interface.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface ICovariantService<out T>
    { }

    /// <summary>
    /// Contravariant generic interface.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal interface IContravariantService<in T>
    { }

    /// <summary>
    /// Generic repository-like interface.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IRepository<T>
    { }

    /// <summary>
    /// Interface for circular dependency tests.
    /// </summary>
    internal interface ICircularDependencyA
    { }

    /// <summary>
    /// Interface for circular dependency tests.
    /// </summary>
    internal interface ICircularDependencyB
    { }

    /// <summary>
    /// Interface used for delegate and factory-related tests.
    /// </summary>
    internal interface IFactoryProduct
    { }

    /// <summary>
    /// Interface for assignable-from tests (marker).
    /// </summary>
    public interface ITestAssignable
    { }

    /// <summary>
    /// Interface derived from <see cref="ITestAssignable"/>.
    /// </summary>
    internal interface ITestAssignableDerived : ITestAssignable
    { }
}
