using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record qefModel
    {
        public int? id { get; set; }
        public int? fId { get; set; }
        public string? featureTitle { get; set; }
        public string? featureTitleAr { get; set; }
        public string? code { get; set; }
        public string? nature { get; set; }
        public string? unit { get; set; }
        public string? datum { get; set; }
        public bool? required { get; set; }
    }
}
