namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class InitialStage : Stage
    {
        public InitialStage(BindingBuilderInternal bindingBuilder, Stage next) : base(bindingBuilder, next, 1)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal)
        { }
    }
}
