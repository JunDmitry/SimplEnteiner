using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Container for nested types used in reflection and assembly scanning tests.
    /// </summary>
    internal static class NestedTypeContainer
    {
        /// <summary>
        /// Nested interface.
        /// </summary>
        internal interface INestedService
        { }

        /// <summary>
        /// Nested implementation of nested interface.
        /// </summary>
        internal class NestedService : INestedService
        { }

        /// <summary>
        /// Nested abstract implementation of nested interface.
        /// </summary>
        internal abstract class NestedAbstractService : INestedService
        { }

        /// <summary>
        /// Nested struct.
        /// </summary>
        internal struct NestedStruct
        { }

        /// <summary>
        /// Nested open generic service implementation.
        /// </summary>
        /// <typeparam name="T">Generic argument.</typeparam>
        internal class NestedGenericService<T> : IGenericService<T>
        { }
    }

    /// <summary>
    /// Container for nested test types.
    /// </summary>
    internal static class TestNestedTypes
    {
        internal interface INestedAssignable { }

        internal class NestedAssignableImpl : INestedAssignable { }

        internal abstract class NestedAssignableAbstract : INestedAssignable { }

        internal class NestedAssignableConcrete : NestedAssignableAbstract { }

        internal sealed class NestedAssignableSealed : INestedAssignable { }

        internal class NestedOpenGeneric<T> : INestedAssignable { }

        internal class NestedClosedGenericString : NestedOpenGeneric<string> { }
    }

    /// <summary>
    /// Container with various nested type visibility levels.
    /// </summary>
    internal class NestedTypeVisibilityContainer
    {
        public static Type PrivateNestedClassType = typeof(PrivateNestedClass);
        public static Type ProtectedNestedClassType = typeof(ProtectedNestedClass);
        public static Type PrivateNestedInterfaceType = typeof(PrivateNestedInterface);

        /// <summary>
        /// Public nested class.
        /// </summary>
        public class PublicNestedClass : AbstractSimpleBase { }

        /// <summary>
        /// Private nested class.
        /// </summary>
        private class PrivateNestedClass  : AbstractSimpleBase
        {
            private PrivateNestedClass() { }
        }

        /// <summary>
        /// Internal nested class.
        /// </summary>
        internal class InternalNestedClass : AbstractSimpleBase { }

        /// <summary>
        /// Protected nested class.
        /// </summary>
        protected class ProtectedNestedClass : AbstractSimpleBase { }

        /// <summary>
        /// Protected internal nested class.
        /// </summary>
        protected internal class ProtectedInternalNestedClass : AbstractSimpleBase { }

        /// <summary>
        /// Public nested struct.
        /// </summary>
        public struct PublicNestedStruct { }

        /// <summary>
        /// Private nested interface.
        /// </summary>
        private interface PrivateNestedInterface { }

        /// <summary>
        /// Deeply nested: class -> class -> class.
        /// </summary>
        public class Level1Nested
        {
            public class Level2Nested
            {
                public class Level3Nested : AbstractSimpleBase
                {
                }
            }
        }

        /// <summary>
        /// Generic nested class.
        /// </summary>
        public class GenericNestedClass<T> { }

        /// <summary>
        /// Closed generic nested class.
        /// </summary>
        public class ClosedNestedGenericClass : GenericNestedClass<int> { }

        /// <summary>
        /// Closed generic nested class.
        /// </summary>
        public class ClosedNestedIsAssignableGenericClass<T> : IGenericServiceDefinition<T> { }
    }
}
