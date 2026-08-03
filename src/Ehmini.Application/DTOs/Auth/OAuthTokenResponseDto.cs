using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public record OAuthTokenResponseDto(
    string access_token,
    string token_type,
    int expires_in,
    string refresh_token
);
}
