namespace SimplEnteinerTests.TestTypes.WithConstraints
{
    /// <summary>
    /// Interface for constraint tests.
    /// </summary>
    internal interface IConstraintTest { }

    /// <summary>
    /// Class implementing IConstraintTest.
    /// </summary>
    internal class ConstraintImplementation : IConstraintTest { }

    /// <summary>
    /// Generic interface with 'class' constraint.
    /// </summary>
    /// <typeparam name="T">Type parameter with class constraint.</typeparam>
    internal interface IGenericWithClassConstraint<T> where T : class
    { }

    /// <summary>
    /// Generic interface with 'struct' constraint.
    /// </summary>
    /// <typeparam name="T">Type parameter with struct constraint.</typeparam>
    internal interface IGenericWithStructConstraint<T> where T : struct
    { }

    /// <summary>
    /// Generic interface with 'new()' constraint.
    /// </summary>
    /// <typeparam name="T">Type parameter with new() constraint.</typeparam>
    internal interface IGenericWithNewConstraint<T> where T : new()
    { }

    /// <summary>
    /// Generic interface with interface constraint.
    /// </summary>
    /// <typeparam name="T">Type parameter constrained to IConstraintTest.</typeparam>
    internal interface IGenericWithInterfaceConstraint<T> where T : IConstraintTest
    { }

    /// <summary>
    /// Generic interface with multiple constraints.
    /// </summary>
    /// <typeparam name="T">Type parameter with multiple constraints.</typeparam>
    internal interface IGenericWithMultipleConstraints<T> where T : class, IConstraintTest, new()
    { }

    /// <summary>
    /// Open generic implementation of IGenericWithClassConstraint.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal class OpenGenericWithClassConstraint<T> : IGenericWithClassConstraint<T> where T : class
    { }

    /// <summary>
    /// Open generic implementation of IGenericWithStructConstraint.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal class OpenGenericWithStructConstraint<T> : IGenericWithStructConstraint<T> where T : struct
    { }

    /// <summary>
    /// Open generic implementation of IGenericWithNewConstraint.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal class OpenGenericWithNewConstraint<T> : IGenericWithNewConstraint<T> where T : new()
    { }

    /// <summary>
    /// Open generic implementation of IGenericWithInterfaceConstraint.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal class OpenGenericWithInterfaceConstraint<T> : IGenericWithInterfaceConstraint<T> where T : IConstraintTest
    { }

    /// <summary>
    /// Open generic implementation of IGenericWithMultipleConstraints.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal class OpenGenericWithMultipleConstraints<T> : IGenericWithMultipleConstraints<T> where T : class, IConstraintTest, new()
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithClassConstraint (valid).
    /// </summary>
    internal class ClosedGenericWithClassConstraint : IGenericWithClassConstraint<string>
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithStructConstraint (valid).
    /// </summary>
    internal class ClosedGenericWithStructConstraint : IGenericWithStructConstraint<int>
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithNewConstraint (valid).
    /// </summary>
    internal class ClosedGenericWithNewConstraint : IGenericWithNewConstraint<ConstraintImplementation>
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithInterfaceConstraint (valid).
    /// </summary>
    internal class ClosedGenericWithInterfaceConstraint : IGenericWithInterfaceConstraint<ConstraintImplementation>
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithMultipleConstraints (valid).
    /// </summary>
    internal class ClosedGenericWithMultipleConstraints : IGenericWithMultipleConstraints<ConstraintImplementation>
    { }

    /// <summary>
    /// Closed generic implementation of IGenericWithNewConstraint (invalid - no parameterless ctor).
    /// </summary>
    internal class ClosedGenericWithInvalidNewConstraint : IGenericWithNewConstraint<ConstraintImplementation>
    { }
}
