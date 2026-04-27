using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with a public settable property.
    /// </summary>
    internal class PropertyInjectionTarget
    {
        public ISimpleService Service { get; set; }
    }

    /// <summary>
    /// Class with a property that has a private setter.
    /// </summary>
    internal class PrivateSetterPropertyInjectionTarget
    {
        public ISimpleService Service { get; private set; }
    }

    /// <summary>
    /// Class with a read-only property.
    /// </summary>
    internal class ReadOnlyPropertyInjectionTarget
    {
        public ISimpleService Service { get; } = null;
    }

    /// <summary>
    /// Class with a public field.
    /// </summary>
    internal class FieldInjectionTarget
    {
        public ISimpleService Service;
    }

    /// <summary>
    /// Class with a private field exposed by a getter method.
    /// </summary>
    internal class PrivateFieldInjectionTarget
    {
        private ISimpleService _service;

        public ISimpleService GetService()
        {
            return _service;
        }
    }
}
