namespace Ehmini.Application.DTOs.AppVersion;

public class AppVersionResponseDto
{
    public string LatestVersion { get; set; } = string.Empty;
    public string MinRequiredVersion { get; set; } = string.Empty;
    public string StoreUrl { get; set; } = string.Empty;
    public string? ReleaseNotes { get; set; }
}
