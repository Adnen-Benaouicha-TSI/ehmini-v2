using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public record ConfirmAccountRequestDto(string Username, string Password, string Code);
}
