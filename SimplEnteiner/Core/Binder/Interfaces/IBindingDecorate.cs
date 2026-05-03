using System;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingDecorate<TInterface>
    {
        IBindingDecorateLifetime<TInterface> With<TImplementation>(int? order = null) where TImplementation : TInterface;
    }

    public interface IBindingDecorate
    {
        IBindingDecorateLifetime With(Type implementation, int? order = null);
    }
}
