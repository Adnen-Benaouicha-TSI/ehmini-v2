using System;
using System.Collections.Generic;

namespace Ehmini.Application.DTOs.Document;

public record DocumentResponseDto(Guid Id, string DocumentType, decimal TotalAmount, string ProviderName, string Reference, DateTime CreatedAt, List<DocumentDetailResponseDto> Details);

public record DocumentDetailResponseDto(Guid Id, string ItemDescription, decimal Quantity, decimal UnitPrice, decimal LineTotal);
