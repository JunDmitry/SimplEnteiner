using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with multiple public constructors, one marked with attribute.
    /// </summary>
    internal class MultipleCtorWithOneAttribute
    {
        public MultipleCtorWithOneAttribute() { }

        [Inject]
        public MultipleCtorWithOneAttribute(ISimpleService service) { }

        public MultipleCtorWithOneAttribute(ISimpleService service, int value) { }
    }

    /// <summary>
    /// Class with multiple public constructors, none marked with attribute.
    /// </summary>
    internal class MultipleCtorNoAttribute
    {
        public MultipleCtorNoAttribute() { }

        public MultipleCtorNoAttribute(ISimpleService service) { }

        public MultipleCtorNoAttribute(ISimpleService service, int value) { }
    }

    /// <summary>
    /// Class with multiple public constructors, multiple marked with attribute (should throw).
    /// </summary>
    internal class MultipleCtorWithMultipleAttributes
    {
        [Inject]
        public MultipleCtorWithMultipleAttributes() { }

        [Inject]
        public MultipleCtorWithMultipleAttributes(ISimpleService service) { }
    }

    /// <summary>
    /// Class with multiple public constructors, one marked with second attribute.
    /// </summary>
    internal class MultipleCtorWithSecondAttribute
    {
        public MultipleCtorWithSecondAttribute() { }

        [SecondInject]
        public MultipleCtorWithSecondAttribute(ISimpleService service) { }
    }
}
