using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.Interfaces;
using Ehmini.Application.Services;
using Ehmini.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Ehmini.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _authService = authService;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("refresh")]
    [AllowAnonymous] // Indispensable car l'ancien AccessToken est expiré
    public async Task<IActionResult> Refresh([FromBody] TokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Une erreur interne est survenue lors du rafraîchissement." });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                id = -2,
                msg = ex.Message
            });
        }
    }

    [Authorize]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            Log.Information("---------------api/Auth/changePassword begin---------------");

            var result = await _authService.ChangePasswordAsync(request);

            Log.Information("---------------api/Auth/changePassword end---------------");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error($"api/Auth/changePassword Error : {ex.Message}");
            Log.Error($"StackTrace : {ex.StackTrace}");

            return Ok(new
            {
                isSucceeded = false,
                msg = ex.Message
            });
        }
    }

    [HttpPost("rrp")]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordDto request)
    {
        try
        {
            Log.Information("---------------api/Auth/rrp begin---------------");
            var result = await _authService.RequestResetPasswordAsync(request);
            Log.Information("---------------api/Auth/rrp end---------------");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error($"api/Auth/rrp Error : {ex.Message}");
            Log.Error($"StackTrace : {ex.StackTrace}");
            return Ok(new RequestResetPasswordResponseDto(-3, ex.Message));
        }
    }

    [HttpPost("rp")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        try
        {
            Log.Information("---------------api/Auth/rp begin---------------");
            var result = await _authService.ResetPasswordAsync(request);
            Log.Information("---------------api/Auth/rp end---------------");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error($"api/Auth/rp Error : {ex.Message}");
            Log.Error($"StackTrace : {ex.StackTrace}");
            return Ok(new ResetPasswordResponseDto(-1, ex.Message));
        }
    }

    [HttpPost("confirmAccount")]
    public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountRequestDto request)
    {
        try
        {
            Log.Information("---------------api/Auth/confirmAccount begin---------------");
            var result = await _authService.ConfirmAccountAsync(request);
            Log.Information("---------------api/Auth/confirmAccount end---------------");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error($"api/Auth/confirmRegistration Error : {ex.Message}");
            Log.Error($"StackTrace : {ex.StackTrace}");
            return Ok(new { id = -1, msg = ex.Message });
        }
    }

    [HttpPost("getConfirmationCode")]
    public async Task<IActionResult> GetConfirmationCode([FromBody] GetConfirmationCodeRequestDto request)
    {
        try
        {
            Log.Information("---------------api/Auth/getConfirmationCode begin---------------");
            var result = await _authService.GetConfirmationCodeAsync(request);
            Log.Information("---------------api/Auth/getConfirmationCode end---------------");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error($"api/Auth/getConfirmationCode Error : {ex.Message}");
            Log.Error($"StackTrace : {ex.StackTrace}");
            return Ok(new { id = -2, msg = ex.Message });
        }
    }

    [HttpPost("updateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserInfoRequestDto request)
    {
        Log.Information("---------------api/Auth/updateUser API triggered---------------");

        var result = await _authService.UpdateUserInfoAsync(request);

        return Ok(result);
    }
    [HttpGet("person")]
    public async Task<IActionResult> GetPerson(CancellationToken cancellationToken)
    {
        try
        {
            var person = await _authService.GetPersonAsync(
                cancellationToken);

            return Ok(new
            {
                isSucceeded = true,
                data = person
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                isSucceeded = false,
                msg = ex.Message
            });
        }
    }
}
