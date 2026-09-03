using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record ProviderQuoteResponseDto(
    string Reference,
    decimal TotalAmount,
    List<QuoteLineDto> Lines,
    DateTime ExpiresAt,
    PhoenixApiResponse reponsePheonix
);
}
