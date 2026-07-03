using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Core.ScopeFactory
{
    public interface IScopeFactory
    {
        IScope CreateScope(IScope parent, ScopeCreationConfig creationConfig);
    }
}
