using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class qModel
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public int userId { get; set; }
        public string reference { get; set; }
        public string a { get; set; }
        public cModel? c { get; set; }
        public string shopCode { get; set; }
        public decimal net { get; set; }
        public decimal fees { get; set; }
        public decimal tax { get; set; }
        public decimal ttc { get; set; }
        public decimal capitaleConstitue { get; set; }
        public int branchId { get; set; }
        public int? benifId { get; set; }
        public ICollection<qeModel>? qeModels { get; set; }
        public ICollection<ptModel>? ptModels { get; set; }
        public ICollection<docModel>? qDocModels { get; set; }
    }
}
