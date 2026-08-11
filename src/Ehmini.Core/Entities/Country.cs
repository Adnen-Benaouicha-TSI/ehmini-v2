using System;
using System.Collections.Generic;

namespace Ehmini.Core.Entities;

public class Country
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DateCreated { get; set; }
    public string? IsoCode { get; set; }


    public ICollection<Region> Regions { get; set; } = new List<Region>();
}