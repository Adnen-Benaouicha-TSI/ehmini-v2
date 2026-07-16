using System;
using System.Collections.Generic;

namespace Ehmini.Core.Entities;

public class Zone
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public DateTime? DateCreated { get; set; }

    public Region Region { get; set; } = null!;
    public ICollection<Locality> Localities { get; set; } = new List<Locality>();
}