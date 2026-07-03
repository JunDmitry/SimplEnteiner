namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class InitialStage : Stage
    {
        public InitialStage(BindingBuilder bindingBuilder, Stage next) : base(bindingBuilder, next, 1)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilder bindingBuilderInternal)
        { }
    }
}
