using System.Threading.Tasks;

namespace SimplEnteiner.Core.Lifecycle
{
    public interface IAsyncInitializable
    {
        Task InitializeAsync();
    }
}
