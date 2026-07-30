using System;

namespace Ehmini.Application.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime Expiration,
    string UserName,
    string Email
);
