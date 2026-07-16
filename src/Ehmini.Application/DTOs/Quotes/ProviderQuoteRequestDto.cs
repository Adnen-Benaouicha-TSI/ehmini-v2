using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record ProviderQuoteRequestDto(
    string CustomerId,
    decimal InsuredValue,
    string RiskCategory,
    Dictionary<string, string> Metadata
);
}
