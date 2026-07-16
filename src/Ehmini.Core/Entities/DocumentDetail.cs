using System;

namespace Ehmini.Core.Entities;

public class DocumentDetail
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public Document? Document { get; set; }
}
