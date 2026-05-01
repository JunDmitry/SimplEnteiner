using System;
using SimplEnteiner.Core.Binder.Interfaces;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.Binder.Implementations
{
    internal class BindingLifetime<TInterface> : IBindingLifetime<TInterface>
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingLifetime(BindingBuilderInternal bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public IBindingOptions<TInterface> AsCached()
        {
            return SetLifetime(LifeTime.Cached);
        }

        public IBindingOptions<TInterface> AsScoped()
        {
            return SetLifetime(LifeTime.Scoped);
        }

        public IBindingOptions<TInterface> AsSingle()
        {
            return SetLifetime(LifeTime.Singleton);
        }

        public IBindingOptions<TInterface> AsTransient()
        {
            return SetLifetime(LifeTime.Transient);
        }

        private IBindingOptions<TInterface> SetLifetime(LifeTime lifeTime)
        {
            _bindingBuilderInternal.SetLifetime(lifeTime);

            return new BindingOptions<TInterface>(_bindingBuilderInternal, _container);
        }
    }

    internal class BindingLifetime : IBindingLifetime
    {
        private readonly BindingBuilderInternal _bindingBuilderInternal;
        private readonly IBindingTarget _container;

        public BindingLifetime(BindingBuilderInternal bindingBuilderInternal, IBindingTarget container)
        {
            _bindingBuilderInternal = bindingBuilderInternal;
            _container = container;
        }

        public IBindingOptions AsCached()
        {
            return SetLifetime(LifeTime.Cached);
        }

        public IBindingOptions AsScoped()
        {
            return SetLifetime(LifeTime.Scoped);
        }

        public IBindingOptions AsSingle()
        {
            return SetLifetime(LifeTime.Singleton);
        }

        public IBindingOptions AsTransient()
        {
            return SetLifetime(LifeTime.Transient);
        }

        private IBindingOptions SetLifetime(LifeTime lifeTime)
        {
            _bindingBuilderInternal.SetLifetime(lifeTime);

            return new BindingOptions(_bindingBuilderInternal, _container);
        }
    }
}
