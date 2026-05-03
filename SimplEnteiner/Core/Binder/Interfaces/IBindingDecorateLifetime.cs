namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBindingDecorateLifetime<TInterface>
    {
        void AsSingle();
        void AsTransient();
        void AsCached();
        void AsScoped();
    }

    public interface IBindingDecorateLifetime
    {
        void AsSingle();
        void AsTransient();
        void AsCached();
        void AsScoped();
    }
}
