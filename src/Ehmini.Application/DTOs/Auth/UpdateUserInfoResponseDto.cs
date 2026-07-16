using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public record UpdateUserInfoResponseDto(bool IsSucceeded,string Msg);
}
