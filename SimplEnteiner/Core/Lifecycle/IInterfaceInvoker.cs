using System.Threading.Tasks;

namespace SimplEnteiner.Core.Lifecycle
{
    internal interface IInterfaceInvoker
    {
        void Invoke<T>(object instance);
        Task InvokeAsync<T>(object instance);
    }
}
