using System;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingOptions<TInterface>
    {
        IBindingOptions<TInterface> WithArguments(params object[] args);
        IBindingOptions<TInterface> WhenInjectedInto<T>();
        IBindingOptions<TInterface> WithId(object id);
        IBindingOptions<TInterface> OnActivation(Action<TInterface> onActivation);
        IBindingOptions<TInterface> OnRelease(Action<TInterface> onRelease);
        void Apply();
    }

    public interface IBindingOptions
    {
        IBindingOptions WithArguments(params object[] args);
        IBindingOptions WhenInjectedInto(Type type);
        IBindingOptions WithId(object id);
        IBindingOptions OnActivation(Action<object> onActivation);
        IBindingOptions OnRelease(Action<object> onRelease);
        void Apply();
    }
}
