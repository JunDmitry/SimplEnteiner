using SimplEnteinerTests.TestTypes.AtributeTypes;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Leaf dependency (no further dependencies).
    /// </summary>
    internal class LeafDependency
    {
        public LeafDependency() { }
    }

    /// <summary>
    /// Middle dependency depending on leaf.
    /// </summary>
    internal class MiddleDependency
    {
        [Inject]
        public MiddleDependency(LeafDependency leaf) { }
    }

    /// <summary>
    /// Root type depending on middle (and indirectly leaf).
    /// </summary>
    internal class RootType
    {
        [Inject]
        public RootType(MiddleDependency middle) { }
    }

    /// <summary>
    /// Type with multiple constructor dependencies.
    /// </summary>
    internal class MultipleDependencies
    {
        [Inject]
        public MultipleDependencies(LeafDependency leaf1, LeafDependency leaf2, MiddleDependency middle) { }
    }

    /// <summary>
    /// Type with injectable field dependency.
    /// </summary>
    internal class FieldDependency
    {
        [InjectField]
        public LeafDependency Leaf;
    }

    /// <summary>
    /// Type with injectable property dependency.
    /// </summary>
    internal class PropertyDependency
    {
        [InjectProperty]
        public LeafDependency Leaf { get; set; }
    }

    /// <summary>
    /// Type with injectable method dependency.
    /// </summary>
    internal class MethodDependency
    {
        [InjectMethod]
        public void SetLeaf(LeafDependency leaf) { }
    }

    /// <summary>
    /// Type with dependency on itself via field (should be excluded from dependencies).
    /// </summary>
    internal class SelfDependencyViaField
    {
        [InjectField]
        public SelfDependencyViaField Self;
    }
}
