using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingDecorateLifetime<TInterface> : IBindingDecorateLifetime<TInterface>
    {
        private readonly BindingBuilder _bindingBuilderInternal;
        private readonly IBindingTarget _target;

        public BindingDecorateLifetime(BindingBuilder bindingBuilderInternal, IBindingTarget target)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _target = target;
        }

        public void AsCached()
        {
            SetLifetime(LifeTime.Cached);
        }

        public void AsScoped()
        {
            SetLifetime(LifeTime.Scoped);
        }

        public void AsSingle()
        {
            SetLifetime(LifeTime.Singleton);
        }

        public void AsTransient()
        {
            SetLifetime(LifeTime.Transient);
        }

        private void SetLifetime(LifeTime lifetime)
        {
            _bindingBuilderInternal.SetLifetime(lifetime);
            Apply();
        }

        public void Apply()
        {
            _target.RegisterDecorator(_bindingBuilderInternal);
        }
    }

    internal class BindingDecorateLifetime : IBindingDecorateLifetime
    {
        private readonly BindingBuilder _bindingBuilderInternal;
        private readonly IBindingTarget _target;

        public BindingDecorateLifetime(BindingBuilder bindingBuilderInternal, IBindingTarget target)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _target = target;
        }

        public void AsCached()
        {
            SetLifetime(LifeTime.Cached);
        }

        public void AsScoped()
        {
            SetLifetime(LifeTime.Scoped);
        }

        public void AsSingle()
        {
            SetLifetime(LifeTime.Singleton);
        }

        public void AsTransient()
        {
            SetLifetime(LifeTime.Transient);
        }

        private void SetLifetime(LifeTime lifetime)
        {
            _bindingBuilderInternal.SetLifetime(lifetime);
            Apply();
        }

        public void Apply()
        {
            _target.RegisterDecorator(_bindingBuilderInternal);
        }
    }
}
