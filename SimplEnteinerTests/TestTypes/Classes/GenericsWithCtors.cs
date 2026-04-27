using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.AtributeTypes;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Generic class with public constructor.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class GenericCtorClass<T>
    {
        public GenericCtorClass() { }

        [Inject]
        public GenericCtorClass(T value) { }
    }

    /// <summary>
    /// Closed generic class inheriting from open generic.
    /// </summary>
    internal class ClosedGenericCtorClass : GenericCtorClass<int>
    {
        public ClosedGenericCtorClass() { }

        [Inject]
        public ClosedGenericCtorClass(int value) : base(value) { }
    }
}
