using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Basic implementation of <see cref="ISimpleService"/>.
    /// </summary>
    public class SimpleService : ISimpleService
    { }

    /// <summary>
    /// First implementation of <see cref="ISimpleService"/> and <see cref="IFirstDerivedSimpleService"/>.
    /// </summary>
    public class FirstSimpleService : IFirstDerivedSimpleService
    { }

    /// <summary>
    /// Second implementation of <see cref="ISimpleService"/> and <see cref="ISecondDerivedSimpleService"/>.
    /// </summary>
    public class SecondSimpleService : ISecondDerivedSimpleService
    { }

    /// <summary>
    /// Concrete class derived from abstract implementation of <see cref="ISimpleService"/>.
    /// </summary>
    internal class DerivedSimpleService : AbstractImplementsSimpleService, IFirstDerivedSimpleService
    { }

    /// <summary>
    /// Implementation of interface with multiple inherited marker interfaces.
    /// </summary>
    internal class MultiInterfaceImplementation : IMultiInterfaceService
    { }

    /// <summary>
    /// Class implementing several unrelated service contracts.
    /// </summary>
    internal class MultiServiceImplementation : ISimpleService, IMarker, IAnotherMarker
    { }

    /// <summary>
    /// Simple implementation of <see cref="IFactoryProduct"/>.
    /// </summary>
    internal class FactoryProduct : IFactoryProduct
    { }

    /// <summary>
    /// Open generic implementation of <see cref="IGenericService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class OpenGenericService<T> : IGenericService<T>
    { }

    /// <summary>
    /// Open generic implementation of <see cref="IGenericDerivedService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class DerivedOpenGenericService<T> : IGenericDerivedService<T>
    { }

    /// <summary>
    /// Closed generic implementation of <see cref="IGenericService{T}"/> for int.
    /// </summary>
    internal class ClosedGenericIntService : IGenericService<int>
    { }

    /// <summary>
    /// Open generic implementation with two generic arguments.
    /// </summary>
    /// <typeparam name="TLeft">Left generic argument.</typeparam>
    /// <typeparam name="TRight">Right generic argument.</typeparam>
    internal class DualGenericService<TLeft, TRight> : IDualGenericService<TLeft, TRight>
    { }

    /// <summary>
    /// Partially closed implementation where second generic argument is fixed to int.
    /// </summary>
    /// <typeparam name="T">First generic argument.</typeparam>
    internal class PartiallyClosedDualGenericService<T> : IDualGenericService<T, int>
    { }

    /// <summary>
    /// Implementation of a covariant generic contract.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class CovariantService<T> : ICovariantService<T>
    { }

    /// <summary>
    /// Implementation of a contravariant generic contract.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ContravariantService<T> : IContravariantService<T>
    { }

    /// <summary>
    /// Open generic implementation of repository contract.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal class Repository<T> : IRepository<T>
    { }

    /// <summary>
    /// Closed generic implementation of repository contract for string.
    /// </summary>
    internal class StringRepository : IRepository<string>
    { }

    /// <summary>
    /// Base class with constructor.
    /// </summary>
    internal class BaseCtorClass
    {
        public BaseCtorClass(ISimpleService service) { }
    }

    /// <summary>
    /// Derived class with constructor calling base.
    /// </summary>
    internal class DerivedCtorClass : BaseCtorClass
    {
        public DerivedCtorClass(ISimpleService service) : base(service) { }
    }

    /// <summary>
    /// Derived class with additional constructor.
    /// </summary>
    internal class DerivedWithExtraCtor : BaseCtorClass
    {
        public DerivedWithExtraCtor(ISimpleService service) : base(service) { }

        [Inject]
        public DerivedWithExtraCtor(ISimpleService service, int value) : base(service) { }
    }
}
