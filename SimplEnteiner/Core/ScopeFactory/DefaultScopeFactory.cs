using SimplEnteiner.Core.ScopeFeature;

namespace SimplEnteiner.Core.ScopeFactory
{
    internal class DefaultScopeFactory : IScopeFactory
    {
        public IScope CreateScope(IScope parent, ScopeCreationConfig creationConfig)
        {
            return new Scope(parent, creationConfig);
        }
    }
}
