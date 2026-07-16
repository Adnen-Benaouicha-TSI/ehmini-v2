using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public record PhoenixApiResponse
    {
        public int id { get; set; }
        public string msg { get; set; } = string.Empty;
        public string track { get; set; } = string.Empty;
        public qModel qModel { get; set; }
        public bool ready { get; set; }
    }
}
