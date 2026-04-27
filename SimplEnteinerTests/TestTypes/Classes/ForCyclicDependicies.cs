using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// First side of interface-based circular dependency.
    /// </summary>
    internal class CircularDependencyA : ICircularDependencyA
    {
        public CircularDependencyA(ICircularDependencyB dependency)
        {
            Dependency = dependency;
        }

        public ICircularDependencyB Dependency { get; }
    }

    /// <summary>
    /// Second side of interface-based circular dependency.
    /// </summary>
    internal class CircularDependencyB : ICircularDependencyB
    {
        public CircularDependencyB(ICircularDependencyA dependency)
        {
            Dependency = dependency;
        }

        public ICircularDependencyA Dependency { get; }
    }

    /// <summary>
    /// First side of concrete-type circular dependency.
    /// </summary>
    internal class CircularConcreteA
    {
        public CircularConcreteA(CircularConcreteB dependency)
        {
            Dependency = dependency;
        }

        public CircularConcreteB Dependency { get; }
    }

    /// <summary>
    /// Second side of concrete-type circular dependency.
    /// </summary>
    internal class CircularConcreteB
    {
        public CircularConcreteB(CircularConcreteA dependency)
        {
            Dependency = dependency;
        }

        public CircularConcreteA Dependency { get; }
    }

    /// <summary>
    /// Self-referencing circular dependency.
    /// </summary>
    internal class SelfCircularDependencyService
    {
        public SelfCircularDependencyService(SelfCircularDependencyService self)
        {
            Self = self;
        }

        public SelfCircularDependencyService Self { get; }
    }

    /// <summary>
    /// Class with a self-referencing constructor dependency (cycle).
    /// </summary>
    internal class SelfCyclicClass
    {
        [Inject]
        public SelfCyclicClass(SelfCyclicClass self) { }
    }

    /// <summary>
    /// Class A with dependency on B.
    /// </summary>
    internal class CycleClassA
    {
        [Inject]
        public CycleClassA(CycleClassB b) { }
    }

    /// <summary>
    /// Class B with dependency on A (creates cycle).
    /// </summary>
    internal class CycleClassB
    {
        [Inject]
        public CycleClassB(CycleClassA a) { }
    }

    /// <summary>
    /// Class with injectable field creating a cycle.
    /// </summary>
    internal class CycleClassWithField
    {
        [InjectField]
        public CycleClassWithField Self;
    }
}
