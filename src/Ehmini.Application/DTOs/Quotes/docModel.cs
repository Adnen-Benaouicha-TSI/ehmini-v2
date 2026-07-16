using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class docModel
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }
}
