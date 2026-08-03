using Ehmini.Application.Interfaces;
using Ehmini.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ehmini.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public string GenerateJwtToken(ApplicationUser user)
    //{
    //    var jwtSettings = _configuration.GetSection("JwtSettings");
    //    var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret absent.");

    //    var claims = new[]
    //    {
    //        new Claim("userId", user.Id.ToString()),
    //        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //        new Claim("unid", user.Cin ?? string.Empty)
    //    };

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: jwtSettings["Issuer"],
    //        audience: jwtSettings["Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddMinutes(15),
    //        signingCredentials: creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}

    public string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret absent.");

        var claims = new[]
        {
            // 💡 CRUCIAL : Standard OAuth2 / OpenID (Sub & NameIdentifier)
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            // Conservé pour rétrocompatibilité interne si nécessaire
            new Claim("userId", user.Id.ToString()),

            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("unid", user.Cin ?? string.Empty),
            new Claim("cin", user.Cin ?? string.Empty),
        };
        //        var claims = new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

        //            // Claims dont vous avez besoin dans Phoenix
        //            new Claim("cin", user.Cin ?? string.Empty),
        //            new Claim("unid", user.Cin ?? string.Empty), // Conservé si utilisé ailleurs
        //            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //};

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 2. Génération d'un Refresh Token Aléatoire et Cryptographique
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    // 3. Récupérer les claims d'un token expiré (indispensable pour le renouvellement)
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret absent.");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // On ne valide pas l'audience ici
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // 💡 TRÈS IMPORTANT : On ignore la date d'expiration pour pouvoir le lire !
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token invalide");

        return principal;
    }
}