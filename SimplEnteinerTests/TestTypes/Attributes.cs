using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplEnteinerTests.TestTypes
{
    /// <summary>
    /// Simple marker attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class MarkerAttribute : Attribute
    { }

    /// <summary>
    /// Attribute with an integer order value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    internal sealed class OrderAttribute : Attribute
    {
        public OrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    /// <summary>
    /// Attribute with a string service key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    internal sealed class ServiceKeyAttribute : Attribute
    {
        public ServiceKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
