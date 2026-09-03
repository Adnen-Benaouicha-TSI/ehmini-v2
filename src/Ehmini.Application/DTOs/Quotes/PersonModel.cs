using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record PersonModel
    {
        public int? personId { get; set; }
        public string? cin { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public int? sex { get; set; }
        public string? birthDate { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public int? countryId { get; set; } = 216;
        public int? locality { get; set; }
        public string? ad { get; set; }
        public int? relationshipId { get; set; }
        public decimal? part { get; set; }
    }
}
