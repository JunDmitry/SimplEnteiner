using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with multiple marked constructors.
    /// </summary>
    internal class MultipleMarkedConstructors
    {
        [Inject]
        public MultipleMarkedConstructors() { }

        [Inject]
        public MultipleMarkedConstructors(int x) { }
    }

    /// <summary>
    /// Class with no marked constructors.
    /// </summary>
    internal class NoMarkedConstructors
    {
        public NoMarkedConstructors() { }
    }

    /// <summary>
    /// Class with one marked constructor.
    /// </summary>
    internal class OneMarkedConstructor
    {
        [Inject]
        public OneMarkedConstructor(ISimpleService service) { }
    }
}
