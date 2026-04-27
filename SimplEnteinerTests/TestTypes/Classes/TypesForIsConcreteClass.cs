namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Open generic base class.
    /// </summary>
    internal class OpenGenericBase<T>
    {
        public OpenGenericBase() { }
    }

    /// <summary>
    /// Closed generic class inheriting from open generic.
    /// This is NOT a generic type itself (IsGenericType == false).
    /// </summary>
    internal class ClosedFromOpenGeneric : OpenGenericBase<int>
    {
        public ClosedFromOpenGeneric() { }
    }

    /// <summary>
    /// Another closed generic inheriting from open generic.
    /// </summary>
    internal class ClosedFromOpenGenericString : OpenGenericBase<string>
    {
        public ClosedFromOpenGenericString() { }
    }

    /// <summary>
    /// Open generic inheriting from open generic (still generic).
    /// </summary>
    internal class OpenFromOpenGeneric<T> : OpenGenericBase<T>
    {
        public OpenFromOpenGeneric() { }
    }

    /// <summary>
    /// Closed generic inheriting from closed generic (NOT generic).
    /// </summary>
    internal class ClosedFromClosedGeneric : ClosedFromOpenGeneric
    {
        public ClosedFromClosedGeneric() { }
    }

    /// <summary>
    /// Abstract closed generic inheriting from open generic.
    /// </summary>
    internal abstract class AbstractClosedFromOpenGeneric : OpenGenericBase<int>
    {
        protected AbstractClosedFromOpenGeneric() { }
    }

    /// <summary>
    /// Concrete class inheriting from abstract closed generic.
    /// </summary>
    internal class ConcreteFromAbstractClosed : AbstractClosedFromOpenGeneric
    {
        public ConcreteFromAbstractClosed() : base() { }
    }
}
