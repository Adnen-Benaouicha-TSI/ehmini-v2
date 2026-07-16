using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ehmini.Application.DTOs.Document;

namespace Ehmini.Application.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> CreateDocumentAsync(Guid userId, CreateDocumentRequestDto dto);
    Task<IEnumerable<DocumentResponseDto>> GetDocumentsByUserAsync(Guid userId);
}
