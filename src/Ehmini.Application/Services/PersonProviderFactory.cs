using Ehmini.Application.Interfaces;
using Ehmini.Application.Interfaces.PersonProviderService;


namespace Ehmini.Application.Services
{
    public class PersonProviderFactory : IPersonProviderFactory
    {
        private readonly IEnumerable<IPersonProvider> _providers;
        private readonly IProviderConfigurationService _configService;

        public PersonProviderFactory(IEnumerable<IPersonProvider> providers, IProviderConfigurationService configService)
        {
            _providers = providers;
            _configService = configService;
        }

        public IPersonProvider GetActiveProvider()
        {
            var activeProviderType = _configService.GetActiveProvider();
            var provider = _providers.FirstOrDefault(p => p.Provider == activeProviderType);

            if (provider == null)
                throw new InvalidOperationException($"Aucun fournisseur de personnes enregistré pour {activeProviderType}");

            return provider;
        }
    }
}
