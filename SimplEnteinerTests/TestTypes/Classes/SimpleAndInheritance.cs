using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Simple class that does not inherit from any custom type and does not implement interfaces.
    /// </summary>
    public class SimpleClass
    { }

    /// <summary>
    /// Class derived from <see cref="SimpleClass"/>.
    /// </summary>
    public class FirstInheritFromSimpleClass : SimpleClass
    { }

    /// <summary>
    /// Class derived from <see cref="SimpleClass"/>.
    /// </summary>
    internal class SecondInheritFromSimpleClass : SimpleClass
    { }

    /// <summary>
    /// Abstract class derived from <see cref="SimpleClass"/>.
    /// </summary>
    public abstract class AbstractInheritFromSimpleClass : SimpleClass
    { }

    /// <summary>
    /// Generic class derived from <see cref="SimpleClass"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class GenericInheritFromSimpleClass<T> : SimpleClass
    { }

    internal class IntegerClosedInheritFromSimleClass : GenericInheritFromSimpleClass<int> { }

    /// <summary>
    /// Sealed class derived from <see cref="SimpleClass"/>.
    /// </summary>
    internal sealed class SealedInheritFromSimpleClass : SimpleClass
    { }

    /// <summary>
    /// Simple generic class.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class GenericClass<T>
    { }

    /// <summary>
    /// Closed generic class based on <see cref="GenericClass{T}"/>.
    /// </summary>
    internal class ClosedGenericStringClass : GenericClass<string>
    { }

    /// <summary>
    /// Concrete implementation of <see cref="AbstractSimpleBase"/>.
    /// </summary>
    internal class ConcreteFromAbstractBase : AbstractSimpleBase
    { }

    /// <summary>
    /// Concrete implementation of <see cref="AbstractGenericBase{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class ConcreteFromAbstractGenericBase<T> : AbstractGenericBase<T>
    { }

    /// <summary>
    /// Static class for negative instantiation tests.
    /// </summary>
    internal static class StaticHelperClass
    { }

    /// <summary>
    /// Concrete class implementing <see cref="ITestAssignable"/>.
    /// </summary>
    public class TestAssignableImpl : ITestAssignable
    { }

    /// <summary>
    /// Abstract class implementing <see cref="ITestAssignable"/>.
    /// </summary>
    internal abstract class TestAssignableAbstractImpl : ITestAssignable
    { }

    /// <summary>
    /// Concrete class derived from <see cref="TestAssignableAbstractImpl"/>.
    /// </summary>
    internal class TestAssignableConcreteFromAbstract : TestAssignableAbstractImpl
    { }

    /// <summary>
    /// Sealed class implementing <see cref="ITestAssignable"/>.
    /// </summary>
    public sealed class TestAssignableSealedImpl : ITestAssignable
    { }

    /// <summary>
    /// Open generic class implementing <see cref="ITestAssignable"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class TestAssignableOpenGeneric<T> : ITestAssignable
    { }

    /// <summary>
    /// Closed generic class implementing <see cref="ITestAssignable"/>.
    /// </summary>
    internal class TestAssignableClosedGenericInt : TestAssignableOpenGeneric<int>
    { }

    /// <summary>
    /// Class that is not assignable to <see cref="ITestAssignable"/>.
    /// </summary>
    internal class TestNotAssignable
    { }

    /// <summary>
    /// Class with a delegate field to ensure delegate types appear in domain types (edge case).
    /// </summary>
    internal class ClassWithDelegateField
    {
        public Action SomeAction;
    }

    /// <summary>
    /// Class used to verify array types are not treated as “non-abstract classes assignable” in a wrong way
    /// (arrays are classes, but usually not the intended targets).
    /// </summary>
    internal class ArrayElementType
    { }
}
