using System.ComponentModel.DataAnnotations;

namespace Ehmini.Application.DTOs.AppVersion;

public class AppVersionUpdateDto
{
    [Required]
    public string Platform { get; set; } = string.Empty; // "android" or "ios"

    [Required]
    public string LatestVersion { get; set; } = string.Empty;

    [Required]
    public string MinRequiredVersion { get; set; } = string.Empty;

    public string StoreUrl { get; set; } = string.Empty;

    public string? ReleaseNotes { get; set; }
}
