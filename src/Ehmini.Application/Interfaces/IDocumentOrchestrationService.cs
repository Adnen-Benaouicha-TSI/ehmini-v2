using Ehmini.Application.DTOs.Document;
using Ehmini.Application.DTOs.Quotes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IDocumentOrchestrationService
    {
        Task<DocumentResponseDto> ProcessAndSaveQuoteAsync(qModel quoteRequest, CancellationToken cancellationToken);
        Task<DocumentResponseDto> UpdateAndSaveQuoteAsync(Guid documentId, qModel quoteRequest, CancellationToken cancellationToken);
        Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken);
    }
}
