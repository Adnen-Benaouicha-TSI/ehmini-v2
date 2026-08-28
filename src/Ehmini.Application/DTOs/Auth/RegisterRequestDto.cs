namespace Ehmini.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Email,
    string Password,
    string Username,
    string FullName,
    string Cin,
    string Phone,
    DateTime BirthDate,
    string CountryId,
    int AddressId = 8,
    int ProfessionId = 1
);


