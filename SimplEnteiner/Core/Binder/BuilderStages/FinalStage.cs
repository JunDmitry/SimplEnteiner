namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class FinalStage : Stage
    {
        public FinalStage(BindingBuilderInternal bindingBuilder, Stage next) : base(bindingBuilder, next, 1000)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal)
        { }
    }
}
