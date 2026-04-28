using System;

namespace SimplEnteiner.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
    }
}
