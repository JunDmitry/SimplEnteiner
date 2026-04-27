namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Interface with a generic constraint for testing.
    /// </summary>
    internal interface IConstraintTest { }

    /// <summary>
    /// Class implementing the constraint interface.
    /// </summary>
    internal class ConstraintImplementation : IConstraintTest { }

    /// <summary>
    /// Class NOT implementing the constraint interface.
    /// </summary>
    internal class NonConstraintImplementation { }
}
