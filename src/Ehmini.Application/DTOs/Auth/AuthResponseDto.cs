using System;

namespace Ehmini.Application.DTOs.Auth;

public record AuthResponseDto(
    int id,
    string msg,
    bool? isConfirmed,
    string? token,
    string? RefreshToken
);
