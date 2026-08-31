using Ehmini.Application.DTOs.Brouillon;
using Ehmini.Application.DTOs.Contracts;
using Ehmini.Application.DTOs.QuotationList;
using Ehmini.Application.DTOs.Quotes;
using Ehmini.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces.QuoteProvider
{
    public interface IQuoteProvider
    {
        ProviderType Provider { get; }
        
        Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel request, CancellationToken cancellationToken);
        Task<ProviderQuoteResponseDto> UpdateQuoteAsync(qModel request, CancellationToken cancellationToken);
        Task<List<ProviderQuotationDto>> GetQuotationsAsync(int language, CancellationToken cancellationToken);
        Task<List<qModel>> GetContractsAsync(int language, CancellationToken cancellationToken);
        Task<List<BrouillonDto>> GetBrouillonsByUserAsync(
         CancellationToken cancellationToken);
    }
}
