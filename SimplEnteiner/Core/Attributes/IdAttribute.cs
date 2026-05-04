using System;

namespace SimplEnteiner.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IdAttribute : Attribute
    {
        public IdAttribute(object id)
        {
            Id = id;
        }

        public object Id { get; }
    }
}
