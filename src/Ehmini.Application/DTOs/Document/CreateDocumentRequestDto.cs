using System.Collections.Generic;

namespace Ehmini.Application.DTOs.Document;

public record CreateDocumentRequestDto(string DocumentType, string ProviderName, string ExternalReference, List<DocumentDetailDto> Details);
