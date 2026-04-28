namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingLifetime<TInterface>
    {
        IBindingOptions<TInterface> AsSingle();
        IBindingOptions<TInterface> AsTransient();
        IBindingOptions<TInterface> AsCached();
        IBindingOptions<TInterface> AsScoped();
    }

    public interface IBindingLifetime
    {
        IBindingOptions AsSingle();
        IBindingOptions AsTransient();
        IBindingOptions AsCached();
        IBindingOptions AsScoped();
    }
}
