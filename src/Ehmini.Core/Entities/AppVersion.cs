using System;
using System.ComponentModel.DataAnnotations;

namespace Ehmini.Core.Entities;

public class AppVersion
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string Platform { get; set; } = string.Empty; // "android" or "ios"

    [Required, MaxLength(20)]
    public string LatestVersion { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string MinRequiredVersion { get; set; } = string.Empty;

    [MaxLength(500)]
    public string StoreUrl { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? ReleaseNotes { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
