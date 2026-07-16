using Ehmini.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Services
{
    public class QuoteProviderFactory : IQuoteProviderFactory
    {
        private readonly IEnumerable<IQuoteProvider> _providers;
        private readonly IProviderConfigurationService _configService;

        public QuoteProviderFactory(IEnumerable<IQuoteProvider> providers, IProviderConfigurationService configService)
        {
            _providers = providers;
            _configService = configService;
        }

        public IQuoteProvider GetActiveProvider()
        {
            var activeProviderType = _configService.GetActiveProvider();
            var provider = _providers.FirstOrDefault(p => p.Provider == activeProviderType);

            if (provider == null)
                throw new InvalidOperationException($"Aucun fournisseur de devis enregistré pour {activeProviderType}");

            return provider;
        }
    }
}
