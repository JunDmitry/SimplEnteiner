using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with one public constructor without attributes.
    /// </summary>
    internal class OnePublicCtorNoAttribute
    {
        public OnePublicCtorNoAttribute() { }
    }

    /// <summary>
    /// Class with one public constructor with attribute.
    /// </summary>
    internal class OnePublicCtorWithAttribute
    {
        [Inject]
        public OnePublicCtorWithAttribute() { }
    }

    /// <summary>
    /// Class with one public constructor with parameters and attribute.
    /// </summary>
    internal class OnePublicCtorWithParamsAndAttribute
    {
        [Inject]
        public OnePublicCtorWithParamsAndAttribute(ISimpleService service, int value) { }
    }

    /// <summary>
    /// Class with one public constructor with parameters but no attribute.
    /// </summary>
    internal class OnePublicCtorWithParamsNoAttribute
    {
        public OnePublicCtorWithParamsNoAttribute(ISimpleService service, int value) { }
    }
}
