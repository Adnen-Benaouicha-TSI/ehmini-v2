using System;
using System.Collections.Generic;
using System.Net;

namespace Ehmini.Core.Entities;

public class Locality
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public DateTime? DateCreated { get; set; }
    public int ZipCode { get; set; }

    public Zone Zone { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}