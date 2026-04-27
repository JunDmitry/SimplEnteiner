using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with injectable fields.
    /// </summary>
    internal class InjectableFieldsClass
    {
        [InjectField]
        public ISimpleService PublicField;

        [InjectField]
        private ISimpleService PrivateField;

        [InjectField]
        internal ISimpleService InternalField;
    }

    /// <summary>
    /// Class with injectable properties.
    /// </summary>
    internal class InjectablePropertiesClass
    {
        [InjectProperty]
        public ISimpleService PublicProperty { get; set; }

        [InjectProperty]
        private ISimpleService PrivateProperty { get; set; }

        [InjectProperty]
        internal ISimpleService InternalProperty { get; set; }
    }

    /// <summary>
    /// Class with injectable methods.
    /// </summary>
    internal class InjectableMethodsClass
    {
        [InjectMethod]
        public void PublicMethod() { }

        [InjectMethod]
        private void PrivateMethod() { }

        [InjectMethod]
        internal void InternalMethod() { }
    }

    /// <summary>
    /// Class with mixed injectable members.
    /// </summary>
    internal class MixedInjectableMembersClass
    {
        [InjectField]
        public ISimpleService Field;

        [InjectProperty]
        public ISimpleService Property { get; set; }

        [InjectMethod]
        public void Method() { }
    }

    /// <summary>
    /// Class without injectable members.
    /// </summary>
    internal class NoInjectableMembersClass
    {
        public ISimpleService Service { get; set; }
        public void DoSomething() { }
    }
}
