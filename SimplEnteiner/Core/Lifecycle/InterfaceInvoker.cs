using System;
using System.Threading.Tasks;

namespace SimplEnteiner.Core.Lifecycle
{
    internal class InterfaceInvoker : IInterfaceInvoker
    {
        public void Invoke<T>(object instance)
        {
            Type type = typeof(T);

            if (type == typeof(IInitializable))
                (instance as IInitializable)?.Initialize();
            else if (type == typeof(IStartable))
                (instance as IStartable)?.Start();
        }

        public async Task InvokeAsync<T>(object instance)
        {
            Type type = typeof(T);

            if (type == typeof(IAsyncInitializable))
                await (instance as IAsyncInitializable)?.InitializeAsync();
        }
    }
}
