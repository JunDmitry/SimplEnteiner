namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class OptionsStage : Stage
    {
        public OptionsStage(BindingBuilderInternal bindingBuilder, Stage next) : base(bindingBuilder, next, 300)
        {
        }

        protected override void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal)
        {
        }
    }
}
