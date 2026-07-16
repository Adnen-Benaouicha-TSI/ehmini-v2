using Ehmini.Core.Entities;
using System.Security.Claims;

namespace Ehmini.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}