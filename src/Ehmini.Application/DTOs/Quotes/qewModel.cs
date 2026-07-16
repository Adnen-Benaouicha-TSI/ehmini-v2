using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record qewModel
    {
        public int id { get; set; }
        public int wId { get; set; }
        public string warrantyTitle { get; set; }
        public string warrantyTitleAr { get; set; }
        public string dStart { get; set; }
        public string dEnd { get; set; }
        public decimal net { get; set; }
        public decimal fees { get; set; }
        public decimal tax { get; set; }
        public decimal ttc { get; set; }
        public decimal? rate { get; set; }
        public string deductible { get; set; }
        public string capital { get; set; }
    }
}
