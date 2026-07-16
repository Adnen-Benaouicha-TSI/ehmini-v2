using Ehmini.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IProviderConfigurationService
    {
        ProviderType GetActiveProvider();
        void SetActiveProvider(ProviderType provider);
    }
}
