using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public record RegisterResponseDto(bool IsSuccess, string Id, string Msg);
}
