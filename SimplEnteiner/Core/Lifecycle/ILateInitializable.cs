using System.Threading.Tasks;

namespace SimplEnteiner.Core.Lifecycle
{
    public interface ILateInitializable
    {
        Task InitializeAsync();
    }
}
