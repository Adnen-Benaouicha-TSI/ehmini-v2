using Ehmini.Application.Interfaces;
using Ehmini.Core.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Ehmini.WebApi.Controllers;

//[ApiController]
[Route("api/providers")]
public class ProvidersAdminController : ControllerBase
{
    private readonly IProviderConfigurationService _configService;

    public ProvidersAdminController(IProviderConfigurationService configService)
    {
        _configService = configService;
    }

    [HttpGet("active")]
    public IActionResult GetActiveProvider()
    {
        return Ok(new { ActiveProvider = _configService.GetActiveProvider().ToString() });
    }

    [HttpPost("switch")]
    public IActionResult SwitchProvider([FromBody] ProviderSwitchRequest request)
    {
        if (!Enum.IsDefined(typeof(ProviderType), request.Provider))
            return BadRequest("Prestataire non valide.");

        _configService.SetActiveProvider(request.Provider);
        return Ok(new { Message = $"Bascule réussie", ActiveProvider = request.Provider.ToString() });
    }
}

public record ProviderSwitchRequest(ProviderType Provider);