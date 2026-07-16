using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record QuoteLineDto(
    string Description,
    decimal LineTotal,
    decimal UnitPrice,
    decimal Quantite
);
}
