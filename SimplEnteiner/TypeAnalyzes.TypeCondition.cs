using System;

namespace SimplEnteiner
{
    public static partial class TypeAnalyzes
    {
        /// <summary>
        /// Contains all the flag properties defined in an instance of type <see cref="System.Type">System.Type</see>
        /// </summary>
        [Flags]
        public enum TypeCondition : long
        {
            None = 0,

            /// <summary>
            /// Value indicating whether the Type is abstract and must be overridden
            /// </summary>
            Abstract = 1L << 0,

            /// <summary>
            /// Value indicating whether the string format attribute AnsiClass is selected for the Type
            /// </summary>
            AnsiClass = 1L << 1,

            /// <summary>
            /// Value that indicates whether the type is an array
            /// </summary>
            Array = 1L << 2,

            /// <summary>
            /// Value indicating whether the string format attribute AutoClass is selected for the Type.
            /// </summary>
            AutoClass = 1L << 3,

            /// <summary>
            /// Value indicating whether the fields of the current type are laid out automatically by the common language runtime.
            /// </summary>
            AutoLayout = 1L << 4,

            /// <summary>
            /// Value indicating whether the Type is passed by reference.
            /// </summary>
            ByRef = 1L << 5,

            /// <summary>
            /// Value that indicates whether the type is a byref-like structure.
            /// </summary>
            ByRefLike = 1L << 6,

            /// <summary>
            /// Value indicating whether the Type is a class or a delegate; that is, not a value type or interface.
            /// </summary>
            Class = 1L << 7,

            /// <summary>
            /// Value indicating whether the Type is a COM object.
            /// </summary>
            COMObject = 1L << 8,

            /// <summary>
            /// Value indicating whether the Type can be hosted in a context.
            /// </summary>
            Contextful = 1L << 9,

            /// <summary>
            /// Value indicating whether the current Type represents an enumeration.
            /// </summary>
            Enum = 1L << 10,

            /// <summary>
            /// Value indicating whether the fields of the current type are laid out at explicitly specified offsets.
            /// </summary>
            ExplicitLayout = 1L << 11,

            /// <summary>
            /// Value that indicates whether the current Type represents a type parameter in the definition of a generic method.
            /// </summary>
            GenericMethodParameter = 1L << 12,

            /// <summary>
            /// Value indicating whether the current Type represents a type parameter in the definition of a generic type or method.
            /// </summary>
            GenericParameter = 1L << 13,

            /// <summary>
            /// Value indicating whether the current type is a generic type.
            /// </summary>
            GenericType = 1L << 14,

            /// <summary>
            /// Value indicating whether the current Type represents a generic type definition, from which other generic types can be constructed.
            /// </summary>
            GenericTypeDefinition = 1L << 15,

            /// <summary>
            /// Value that indicates whether the current Type represents a type parameter in the definition of a generic type.
            /// </summary>
            GenericTypeParameter = 1L << 16,

            /// <summary>
            /// Value indicating whether the Type has a ComImportAttribute attribute applied, indicating that it was imported from a COM type library.
            /// </summary>
            Import = 1L << 17,

            /// <summary>
            /// Value indicating whether the Type is an interface; that is, not a class or a value type.
            /// </summary>
            Interface = 1L << 18,

            /// <summary>
            /// Value indicating whether the fields of the current type are laid out sequentially, in the order that they were defined or emitted to the metadata.
            /// </summary>
            LayoutSequential = 1L << 19,

            /// <summary>
            /// Value indicating whether the Type is marshaled by reference.
            /// </summary>
            MarshalByRef = 1L << 20,

            /// <summary>
            /// Value indicating whether the current Type object represents a type whose definition is nested inside the definition of another type.
            /// </summary>
            Nested = 1L << 21,

            /// <summary>
            /// Value indicating whether the Type is nested and visible only within its own assembly.
            /// </summary>
            NestedAssembly = 1L << 22,

            /// <summary>
            /// Value indicating whether the Type is nested and visible only to classes that belong to both its own family and its own assembly.
            /// </summary>
            NestedFamAndAssem = 1L << 23,

            /// <summary>
            /// Value indicating whether the Type is nested and visible only within its own family.
            /// </summary>
            NestedFamily = 1L << 24,

            /// <summary>
            /// Value indicating whether the Type is nested and visible only to classes that belong to either its own family or to its own assembly.
            /// </summary>
            NestedFamOrAssem = 1L << 25,

            /// <summary>
            /// Value indicating whether a class is nested and declared public.
            /// </summary>
            NestedPublic = 1L << 26,

            /// <summary>
            /// Value indicating whether the Type is nested and declared private.
            /// </summary>
            NestedPrivate = 1L << 27,

            /// <summary>
            /// Value indicating whether the Type is not declared public.
            /// </summary>
            NotPublic = 1L << 28,

            /// <summary>
            /// Value indicating whether the Type is a pointer.
            /// </summary>
            Pointer = 1L << 29,

            /// <summary>
            /// Value indicating whether the Type is one of the primitive types.
            /// </summary>
            Primitive = 1L << 30,

            /// <summary>
            /// Value indicating whether the Type is declared public.
            /// </summary>
            Public = 1L << 31,

            /// <summary>
            /// Value indicating whether the Type is declared sealed.
            /// </summary>
            Sealed = 1L << 32,

            /// <summary>
            /// Value that indicates whether the current type is security-critical or security-safe-critical <br/>
            /// at the current trust level, and therefore can perform critical operations.
            /// </summary>
            SecurityCritical = 1L << 33,

            /// <summary>
            /// Value that indicates whether the current type is security-safe-critical at the current trust level; <br/>
            /// that is, whether it can perform critical operations and can be accessed by transparent code.
            /// </summary>
            SecuritySafeCritical = 1L << 34,

            /// <summary>
            /// Value that indicates whether the current type is transparent at the current trust level, and therefore cannot perform critical operations.
            /// </summary>
            SecurityTransparent = 1L << 35,

            /// <summary>
            /// Value indicating whether the Type is binary serializable.
            /// </summary>
            [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
            Serializable = 1L << 36,

            /// <summary>
            /// Value that indicates whether the type is a signature type.
            /// </summary>
            SignatureType = 1L << 37,

            /// <summary>
            /// Value indicating whether the type has a name that requires special handling.
            /// </summary>
            SpecialName = 1L << 38,

            /// <summary>
            /// Value that indicates whether the type is an array type that can represent only a single-dimensional array with a zero lower bound.
            /// </summary>
            SZArray = 1L << 39,

            /// <summary>
            /// Value that indicates whether the type is a type definition.
            /// </summary>
            TypeDefinition = 1L << 40,

            /// <summary>
            /// Value indicating whether the string format attribute UnicodeClass is selected for the Type.
            /// </summary>
            UnicodeClass = 1L << 41,

            /// <summary>
            /// Value indicating whether the Type is a value type.
            /// </summary>
            ValueType = 1L << 42,

            /// <summary>
            /// Value that indicates whether the type is an array type that can represent a multi-dimensional array or an array with an arbitrary lower bound.
            /// </summary>
            VariableBoundArray = 1L << 43,

            /// <summary>
            /// Value indicating whether the Type can be accessed by code outside the assembly.
            /// </summary>
            Visible = 1L << 44,

            /// <summary>
            /// Value that indicates whether this object represents a constructed generic type.
            /// </summary>
            ConstructedGenericType = 1L << 45,
        }
    }
}
