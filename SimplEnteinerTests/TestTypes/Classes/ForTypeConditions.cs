namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Sealed class for IsSealed condition tests.
    /// </summary>
    internal sealed class SealedTestClass
    {
        public SealedTestClass() { }
    }

    /// <summary>
    /// Non-sealed class for IsSealed condition tests.
    /// </summary>
    internal class NonSealedTestClass
    {
        public NonSealedTestClass() { }
    }

    /// <summary>
    /// Value type struct for IsValueType condition tests.
    /// </summary>
    internal struct ValueTypeStruct : IConvertible
    {
        public int Value;

        public TypeCode GetTypeCode()
        {
            return Value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToByte(provider);
        }

        public char ToChar(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToSingle(provider);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            return ((IConvertible)Value).ToUInt64(provider);
        }
    }

    /// <summary>
    /// Enum for IsEnum condition tests.
    /// </summary>
    internal enum TestEnum
    {
        First,
        Second,
        Third
    }

    /// <summary>
    /// Public class for IsPublic condition tests.
    /// </summary>
    public class PublicTestClass
    {
        public PublicTestClass() { }
    }

    /// <summary>
    /// Internal class for IsPublic condition tests (should not match).
    /// </summary>
    internal class InternalTestClass
    {
        internal InternalTestClass() { }
    }

    /// <summary>
    /// Class with nested public type for IsNestedPublic condition.
    /// </summary>
    internal class ContainerWithNestedPublic
    {
        public class NestedPublicType { }
    }

    /// <summary>
    /// Class with nested private type for IsNestedPrivate condition.
    /// </summary>
    internal class ContainerWithNestedPrivate
    {
        private class NestedPrivateType { }
    }
}
