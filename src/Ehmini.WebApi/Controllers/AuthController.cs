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
    private readonly IPkceService _pkceService;

    public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, ITokenService tokenService, IPkceService pkceService)
    {
        _authService = authService;
        _userManager = userManager;
        _tokenService = tokenService;
        _pkceService = pkceService;
    }

    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded", "application/json")]
    public async Task<IActionResult> Token([FromForm][FromBody] OAuthTokenRequestDto request)
    {
        // -----------------------------------------------------------------
        // 1. GRANT TYPE: PASSWORD WITH PKCE
        // -----------------------------------------------------------------
        if (string.Equals(request.GrantType, "password", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "invalid_request", error_description = "Username et Password requis." });
            }

            // Exigence PKCE : code_verifier et code_challenge sont obligatoires
            if (string.IsNullOrEmpty(request.CodeVerifier) || string.IsNullOrEmpty(request.CodeChallenge))
            {
                return BadRequest(new
                {
                    error = "invalid_request",
                    error_description = "PKCE requis : 'code_verifier' et 'code_challenge' doivent être fournis."
                });
            }

            // Validation mathématique du PKCE
            bool isPkceValid = _pkceService.ValidateCodeVerifier(
                request.CodeVerifier,
                request.CodeChallenge,
                request.CodeChallengeMethod ?? "S256"
            );

            if (!isPkceValid)
            {
                return BadRequest(new
                {
                    error = "invalid_grant",
                    error_description = "Échec de validation PKCE : 'code_verifier' invalide."
                });
            }

            // Vérification des identifiants utilisateur (Email / Mot de passe)
            try
            {
                var loginDto = new LoginRequestDto(request.Username, request.Password);
                var result = await _authService.LoginAsync(loginDto);

                return Ok(new OAuthTokenResponseDto(
                    access_token: result.AccessToken,
                    token_type: "Bearer",
                    expires_in: 15 * 60,
                    refresh_token: result.RefreshToken
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { error = "invalid_grants", error_description = ex.Message });
            }
        }
        // -----------------------------------------------------------------
        // 2. GRANT TYPE: REFRESH TOKEN
        // -----------------------------------------------------------------
        else if (string.Equals(request.GrantType, "refresh_token", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { error = "invalid_request", error_description = "RefreshToken requis." });
            }

            try
            {
                var tokenDto = new TokenRequestDto { AccessToken = string.Empty, RefreshToken = request.RefreshToken };
                var result = await _authService.RefreshAsync(tokenDto);

                return Ok(new OAuthTokenResponseDto(
                    access_token: result.AccessToken,
                    token_type: "Bearer",
                    expires_in: 15 * 60,
                    refresh_token: result.RefreshToken
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "invalid_grant", error_description = ex.Message });
            }
        }

        return BadRequest(new { error = "unsupported_grant_type", error_description = "Grant type non supporté." });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
