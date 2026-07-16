using System;

namespace Ehmini.Core.Entities;

public class Address
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int LocalityId { get; set; }
    public DateTime? DateCreated { get; set; }

    public Locality Locality { get; set; } = null!;
}