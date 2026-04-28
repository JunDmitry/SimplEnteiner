using SimplEnteiner.Core.LifeScope;

namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class LifetimeStage : Stage
    {
        public LifetimeStage(BindingBuilderInternal bindingBuilder, Stage next) : base(bindingBuilder, next, 200)
        {
        }

        protected override void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal)
        {
            bindingBuilderInternal.SetLifetime(LifeTime.Transient);
        }
    }
}
