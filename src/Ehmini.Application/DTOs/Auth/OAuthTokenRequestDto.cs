using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public class OAuthTokenRequestDto
    {
        // "password" ou "refresh_token"
        public string GrantType { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public string? CodeVerifier { get; set; }
        public string? CodeChallenge { get; set; }
        public string? CodeChallengeMethod { get; set; }
    }
}
