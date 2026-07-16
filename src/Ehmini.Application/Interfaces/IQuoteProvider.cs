using Ehmini.Application.DTOs.Quotes;
using Ehmini.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IQuoteProvider
    {
        ProviderType Provider { get; }
        
        Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel request, CancellationToken cancellationToken);
        Task<ProviderQuoteResponseDto> UpdateQuoteAsync(qModel request, CancellationToken cancellationToken);
    }
}
