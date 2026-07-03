namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class OptionsStage : Stage
    {
        public OptionsStage(BindingBuilder bindingBuilder, Stage next) : base(bindingBuilder, next, 300)
        {
        }

        protected override void OnExecuteBinding(BindingBuilder bindingBuilderInternal)
        {
        }
    }
}
