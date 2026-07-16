using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class cModel
    {
        public int id { get; set; }
        public qModel? quotation { get; set; }
        public int version { get; set; }
        public string reference { get; set; }
    }
}
