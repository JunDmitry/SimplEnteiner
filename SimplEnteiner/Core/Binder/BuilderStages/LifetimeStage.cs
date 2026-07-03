using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class LifetimeStage : Stage
    {
        public LifetimeStage(BindingBuilder bindingBuilder, Stage next) : base(bindingBuilder, next, 200)
        {
        }

        protected override void OnExecuteBinding(BindingBuilder bindingBuilderInternal)
        {
            bindingBuilderInternal.SetLifetime(LifeTime.Transient);
        }
    }
}
