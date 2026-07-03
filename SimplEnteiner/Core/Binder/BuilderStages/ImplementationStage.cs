namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class ImplementationStage : Stage
    {
        public ImplementationStage(BindingBuilder bindingBuilder, Stage next) : base(bindingBuilder, next, 100)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilder bindingBuilderInternal)
        {
            bindingBuilderInternal.SetImplementation(bindingBuilderInternal.InterfaceType);
        }
    }
}
