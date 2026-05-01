using System;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingDecorate<TInterface>
    {
        IBindingDecorate<TInterface> With<TImplementation>(int? order = null) where TImplementation : TInterface;
        void Apply();
    }

    public interface IBindingDecorate
    {
        IBindingDecorate With(Type implementation, int? order = null);
        void Apply();
    }
}
