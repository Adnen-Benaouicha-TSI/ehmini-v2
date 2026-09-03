using Ehmini.Application.DTOs.Brouillon;
using Ehmini.Application.DTOs.Contracts;
using Ehmini.Application.DTOs.Document;
using Ehmini.Application.DTOs.QuotationList;
using Ehmini.Application.DTOs.Quotes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IDocumentOrchestrationService
    {
        Task<PhoenixApiResponse> ProcessAndSaveQuoteAsync(qModel quoteRequest, Guid userIdClaim, CancellationToken cancellationToken);
        Task<PhoenixApiResponse> UpdateAndSaveQuoteAsync(string reference, qModel quoteRequest, CancellationToken cancellationToken);
        Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken);
        Task<List<ProviderQuotationDto>> GetQuotationsAsync(int language,CancellationToken cancellationToken);
        Task<List<qModel>> GetContractsAsync(int language, CancellationToken cancellationToken);
        Task<List<BrouillonDto>> GetBrouillonsByUserAsync(CancellationToken cancellationToken);
    }
}
