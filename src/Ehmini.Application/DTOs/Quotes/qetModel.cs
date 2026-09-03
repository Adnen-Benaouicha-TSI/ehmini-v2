using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record qetModel
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public decimal? amount { get; set; }
    }
}
