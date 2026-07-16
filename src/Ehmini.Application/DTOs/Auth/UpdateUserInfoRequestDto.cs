using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Auth
{
    public record UpdateUserInfoRequestDto(
    string Cin,
    string? FullName,
    string? Email,
    string? Phone,
    string? CountryIsoCode,
    string? Signature,
    DateTime? Birthday,
    int ProfessionId,
    int? AddressId);
}
