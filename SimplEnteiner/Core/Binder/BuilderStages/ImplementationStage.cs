namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class ImplementationStage : Stage
    {
        public ImplementationStage(BindingBuilderInternal bindingBuilder, Stage next) : base(bindingBuilder, next, 100)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal)
        {
            bindingBuilderInternal.SetImplementation(bindingBuilderInternal.InterfaceType);
        }
    }
}
