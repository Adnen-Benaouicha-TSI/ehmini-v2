using Ehmini.Core.Entities;
using System.Security.Claims;

namespace Ehmini.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}