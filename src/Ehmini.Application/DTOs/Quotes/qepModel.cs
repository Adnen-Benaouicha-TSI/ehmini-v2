using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record qepModel
    {
        public int id { get; set; }
        public decimal net { get; set; }
        public decimal ttc { get; set; }
        public int rank { get; set; }
        public string dStart { get; set; }
        public string dEnd { get; set; }
    }
}
