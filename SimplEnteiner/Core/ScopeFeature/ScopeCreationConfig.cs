using SimplEnteiner.Core.RegistrationService.Factory;
using SimplEnteiner.Core.RepositoryService;
using SimplEnteiner.Core.ResolverService;
using SimplEnteiner.Core.ScopeFactory;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ScopeFeature
{
    public sealed class ScopeCreationConfig
    {
        private IResolver _resolver = new Resolver();
        private IRegistryFactory _registryFactory = new RegistryFactory();
        private IScopeFactory _scopeFactory = new DefaultScopeFactory();
        private IRepositoryService _singletonRepository;

        public IResolver Resolver
        {
            get => _resolver;
            set => _resolver = value.ThrowIfArgumentNull();
        }
        public IRegistryFactory RegistryFactory
        {
            get => _registryFactory;
            set => _registryFactory = value.ThrowIfArgumentNull();
        }
        public IScopeFactory ScopeFactory
        {
            get => _scopeFactory;
            set => _scopeFactory = value.ThrowIfArgumentNull();
        }

        public IRepositoryService SingletonRepository
        {
            get => _singletonRepository;
            set => _singletonRepository = value.ThrowIfArgumentNull();
        }
    }
}
