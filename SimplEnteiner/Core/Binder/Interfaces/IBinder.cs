using System;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    public interface IBinder
    {
        public IBindingTo<T> Bind<T>();

        public IBindingTo Bind(Type interfaceType);
        IBindingDecorate<TService> Decorate<TService>();
        IBindingDecorate Decorate(Type interfaceType);
    }
}
