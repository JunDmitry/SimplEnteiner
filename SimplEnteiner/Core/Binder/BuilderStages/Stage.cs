namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal abstract class Stage
    {
        private readonly BindingBuilderInternal _bindingBuilder;

        protected Stage(BindingBuilderInternal bindingBuilder, Stage next, int id)
        {
            _bindingBuilder = bindingBuilder;
            Next = next;
            Id = id;
        }

        public Stage Next { get; }
        public int Id { get; }

        public void Execute()
        {
            OnExecuteBinding(_bindingBuilder);
        }

        protected abstract void OnExecuteBinding(BindingBuilderInternal bindingBuilderInternal); 
    }
}
