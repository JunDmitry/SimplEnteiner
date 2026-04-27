using System.Runtime.CompilerServices;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with compiler-generated nested type (iterator/state machine).
    /// </summary>
    internal class ClassWithCompilerGeneratedNested
    {
        public IEnumerable<int> GetNumbers()
        {
            yield return 1;
            yield return 2;
        }
    }

    /// <summary>
    /// Class with auto-property (compiler-generated backing field).
    /// </summary>
    internal class ClassWithAutoProperty
    {
        public int AutoProp { get; set; }
    }

    /// <summary>
    /// Class with explicit compiler-generated attribute.
    /// </summary>
    [CompilerGenerated]
    internal class ExplicitlyGeneratedClass
    {
    }

    /// <summary>
    /// Partial class to simulate compiler-generated scenarios.
    /// </summary>
    internal partial class PartialClassOne
    {
        public int Value { get; set; }
    }

    internal partial class PartialClassOne
    {
        public string Name { get; set; }
    }
}
