namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class FinalStage : Stage
    {
        public FinalStage(BindingBuilder bindingBuilder, Stage next) : base(bindingBuilder, next, 1000)
        { }

        protected sealed override void OnExecuteBinding(BindingBuilder bindingBuilderInternal)
        {
        }
    }
}
