using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Static class (should not be concrete).
    /// </summary>
    internal static class AnotherStaticClass
    {
        public static void DoSomething() { }
    }

    /// <summary>
    /// Non-static class with static members.
    /// </summary>
    internal class ClassWithStaticMembers
    {
        public static int StaticField = 42;

        public ClassWithStaticMembers() { }
    }
}
