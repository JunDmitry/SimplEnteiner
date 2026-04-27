using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplEnteinerTests.TestTypes
{
    /// <summary>
    /// Simple enum for primitive registration or metadata tests.
    /// </summary>
    internal enum SimpleEnum
    {
        None = 0,
        First = 1,
        Second = 2
    }

    /// <summary>
    /// Flags enum for bitwise value tests.
    /// </summary>
    [Flags]
    internal enum TestFlagsEnum
    {
        None = 0,
        First = 1,
        Second = 2,
        All = First | Second
    }
}
