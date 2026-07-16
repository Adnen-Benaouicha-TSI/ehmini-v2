using System;
using System.Collections.Generic;

namespace Ehmini.Core.Entities;

public class Region
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public DateTime? DateCreated { get; set; }

    public Country Country { get; set; } = null!;
    public ICollection<Zone> Zones { get; set; } = new List<Zone>();
}