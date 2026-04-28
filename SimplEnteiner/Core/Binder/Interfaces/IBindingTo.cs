using System;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingTo<TInterface>
    {
        IBindingLifetime<TInterface> To<TImplementation>() where TImplementation : TInterface;
        IBindingLifetime<TInterface> ToSelf();
        IBindingLifetime<TInterface> ToMethod(Func<TInterface> factory);
        IBindingLifetime<TInterface> ToInstance(TInterface instance);
    }

    public interface IBindingTo
    {
        IBindingLifetime To(Type implementation);
        IBindingLifetime ToSelf();
        IBindingLifetime ToMethod(Func<object> factory);
        IBindingLifetime ToInstance(object instance);
    }
}
