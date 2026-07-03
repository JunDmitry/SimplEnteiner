using System;
using System.Collections.Generic;

namespace SimplEnteiner.Core.ScopeFeature
{
    public struct ConditionalKey
    {
        public Type interfaceType;
        public object id;

        public ConditionalKey(Type interfaceType, object id)
        {
            this.interfaceType = interfaceType;
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is ConditionalKey other &&
                   EqualityComparer<Type>.Default.Equals(interfaceType, other.interfaceType) &&
                   EqualityComparer<object>.Default.Equals(id, other.id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(interfaceType, id);
        }

        public void Deconstruct(out Type interfaceType, out object id)
        {
            interfaceType = this.interfaceType;
            id = this.id;
        }

        public static implicit operator (Type interfaceType, object id)(ConditionalKey value)
        {
            return (value.interfaceType, value.id);
        }

        public static implicit operator ConditionalKey((Type interfaceType, object id) value)
        {
            return new ConditionalKey(value.interfaceType, value.id);
        }
    }
}
