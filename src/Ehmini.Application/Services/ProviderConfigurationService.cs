using Ehmini.Application.Interfaces;
using Ehmini.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Services
{
    public class ProviderConfigurationService : IProviderConfigurationService
    {
        private ProviderType _activeProvider = ProviderType.Phoenix;

        public ProviderType GetActiveProvider() => _activeProvider;
        public void SetActiveProvider(ProviderType provider) => _activeProvider = provider;
    }
}
