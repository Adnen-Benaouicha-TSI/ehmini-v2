namespace Ehmini.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Email,
    string Password,
    string Username,
    string FullName,
    string Cin,
    string Phone,
    DateTime BirthDate,
    int CountryId,
    int AddressId,
    int ProfessionId
);


