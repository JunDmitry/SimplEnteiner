using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Concrete class with no dependencies.
    /// </summary>
    internal class ConcreteNoDependencies
    {
        public ConcreteNoDependencies() { }
    }

    /// <summary>
    /// Concrete class with dependency on another concrete class.
    /// </summary>
    internal class ConcreteWithConcreteDependency
    {
        [Inject]
        public ConcreteWithConcreteDependency(ConcreteNoDependencies dependency) { }
    }

    /// <summary>
    /// Concrete class with dependency on interface (not registered).
    /// </summary>
    internal class ConcreteWithUnregisteredInterfaceDependency
    {
        [Inject]
        public ConcreteWithUnregisteredInterfaceDependency(ISimpleService service) { }
    }

    /// <summary>
    /// Concrete class with dependency on interface (registered).
    /// </summary>
    internal class ConcreteWithRegisteredInterfaceDependency
    {
        [Inject]
        public ConcreteWithRegisteredInterfaceDependency(ISimpleService service) { }
    }

    /// <summary>
    /// Concrete class with multiple dependencies.
    /// </summary>
    internal class ConcreteWithMultipleDependencies
    {
        [Inject]
        public ConcreteWithMultipleDependencies(
            ConcreteNoDependencies dep1,
            ISimpleService dep2,
            ConcreteWithConcreteDependency dep3)
        { }
    }

    /// <summary>
    /// Abstract class with dependency.
    /// </summary>
    internal abstract class AbstractWithDependency
    {
        protected AbstractWithDependency(ISimpleService service) { }
    }

    /// <summary>
    /// Concrete class inheriting from abstract with dependency.
    /// </summary>
    internal class ConcreteFromAbstractWithDependency : AbstractWithDependency
    {
        public ConcreteFromAbstractWithDependency(ISimpleService service) : base(service) { }
    }
}
