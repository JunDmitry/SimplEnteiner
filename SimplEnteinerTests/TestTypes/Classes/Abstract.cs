using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Simple abstract base class.
    /// </summary>
    internal abstract class AbstractSimpleBase
    { }

    /// <summary>
    /// Abstract class implementing <see cref="ISimpleService"/>.
    /// </summary>
    internal abstract class AbstractImplementsSimpleService : ISimpleService
    { }

    /// <summary>
    /// Simple abstract generic base class.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal abstract class AbstractGenericBase<T>
    { }

    /// <summary>
    /// Abstract open generic implementation of <see cref="IGenericService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal abstract class AbstractOpenGenericService<T> : IGenericService<T>
    { }

    /// <summary>
    /// Abstract class with a constructor dependency.
    /// </summary>
    internal abstract class AbstractClassWithDependency
    {
        protected AbstractClassWithDependency(ISimpleService service)
        {
            Service = service;
        }

        public ISimpleService Service { get; }
    }

    /// <summary>
    /// Interface for IsConcreteClass tests.
    /// </summary>
    internal interface IConcreteClassTest
    {
        void DoWork();
    }

    /// <summary>
    /// Abstract class implementing interface.
    /// </summary>
    internal abstract class AbstractImplementingInterface : IConcreteClassTest
    {
        public abstract void DoWork();
    }

    /// <summary>
    /// Concrete implementation.
    /// </summary>
    internal class ConcreteImplementingInterface : AbstractImplementingInterface
    {
        public override void DoWork() { }
    }

    /// <summary>
    /// Abstract class without interface.
    /// </summary>
    internal abstract class PlainAbstractClass
    {
        protected PlainAbstractClass() { }

        public abstract void Method();
    }
}
