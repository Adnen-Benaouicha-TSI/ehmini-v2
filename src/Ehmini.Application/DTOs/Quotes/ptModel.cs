using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class ptModel
    {
        public int id { get; set; }
        public decimal total { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public bool active { get; set; }
        public string dDate { get; set; }
        public string dPaid { get; set; }
        public string dateEnd { get; set; }
    }
}
