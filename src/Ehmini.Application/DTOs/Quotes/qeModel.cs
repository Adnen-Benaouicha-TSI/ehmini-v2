using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class qeModel
    {
        public int? id { get; set; }
        public int? productId { get; set; }
        public string? productTitle { get; set; }
        public string? productTitleAr { get; set; }
        public string? branchTitle { get; set; }
        public string? branchTitleAr { get; set; }
        public string? principal { get; set; }
        public string? dStart { get; set; }
        public string? dEnd { get; set; }
        public int? dId { get; set; }
        public int? pId { get; set; }
        public decimal? net { get; set; }
        public decimal? fees { get; set; }
        public decimal? tax { get; set; }
        public decimal? ttc { get; set; }
        public ICollection<qewModel>? qewModels { get; set; }
        public ICollection<qefModel>? qefModels { get; set; }
        public ICollection<qefeModel>? qefeModels { get; set; }
        public ICollection<qetModel>? qetModels { get; set; }
        public ICollection<qepModel>? qepModels { get; set; }
        public ICollection<docModel>? qeDocModels { get; set; }
        public ICollection<PersonModel>? personModel { get; set; }
        public bool? ready { get; set; }
        public bool? reconductible { get; set; }
    }
}
