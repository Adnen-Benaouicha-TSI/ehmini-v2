using System;
using System.Threading.Tasks;
using Ehmini.Application.DTOs.AppVersion;
using Ehmini.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppVersionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppVersionController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Public endpoint — no auth required.
    /// Returns the latest version info for the given platform.
    /// GET /api/AppVersion/latest?platform=android
    /// GET /api/AppVersion/latest?platform=ios
    /// </summary>
    [HttpGet("latest")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatest([FromQuery] string platform)
    {
        if (string.IsNullOrWhiteSpace(platform) ||
            (platform != "android" && platform != "ios"))
        {
            return BadRequest(new { message = "Platform must be 'android' or 'ios'." });
        }

        var record = await _context.AppVersions
            .FirstOrDefaultAsync(v => v.Platform == platform.ToLower());

        if (record == null)
        {
            return NotFound(new { message = $"No version record found for platform '{platform}'." });
        }

        return Ok(new AppVersionResponseDto
        {
            LatestVersion = record.LatestVersion,
            MinRequiredVersion = record.MinRequiredVersion,
            StoreUrl = record.StoreUrl,
            ReleaseNotes = record.ReleaseNotes
        });
    }

    /// <summary>
    /// Admin-protected endpoint to update the version record for a platform.
    /// PUT /api/AppVersion/update
    /// </summary>
    [HttpPut("update")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> UpdateVersion([FromBody] AppVersionUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var platform = dto.Platform.ToLower();
        if (platform != "android" && platform != "ios")
            return BadRequest(new { message = "Platform must be 'android' or 'ios'." });

        var record = await _context.AppVersions
            .FirstOrDefaultAsync(v => v.Platform == platform);

        if (record == null)
        {
            // Create new record if not found (safety net — seed should always provide rows)
            record = new AppVersion { Platform = platform };
            _context.AppVersions.Add(record);
        }

        record.LatestVersion = dto.LatestVersion;
        record.MinRequiredVersion = dto.MinRequiredVersion;
        record.StoreUrl = dto.StoreUrl;
        record.ReleaseNotes = dto.ReleaseNotes;
        record.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = $"Version for '{platform}' updated successfully.", version = record.LatestVersion });
    }
}
