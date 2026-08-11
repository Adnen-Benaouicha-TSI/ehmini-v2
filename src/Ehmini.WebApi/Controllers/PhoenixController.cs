using Ehmini.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ehmini.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PhoenixController : ControllerBase
{
    private readonly IPhoenixTokenService _phoenixTokenService;

    public PhoenixController(IPhoenixTokenService phoenixTokenService)
    {
        _phoenixTokenService = phoenixTokenService;
    }

    [HttpGet("token")]
    public async Task<IActionResult> GetToken(CancellationToken cancellationToken)
    {
        var cin = User.FindFirst("cin")?.Value;

        if (string.IsNullOrEmpty(cin))
        {
            return Unauthorized(new { message = "cin introuvable dans le token utilisateur." });
        }

        var accessToken = await _phoenixTokenService.GetPhoenixTokenAsync(cin, cancellationToken);

        return Ok(new { access_token = accessToken });
    }
}