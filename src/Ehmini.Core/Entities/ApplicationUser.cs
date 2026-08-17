using System;
using Microsoft.AspNetCore.Identity;

namespace Ehmini.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cin { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int CountryId { get; set; }
    public int Status { get; set; }
    public bool IsAccountConfirmed { get; set; }
    public string? AccountConfirmationToken { get; set; }
    public DateTime? PwdResetTokenCreationDate { get; set; }
    public string? Signature { get; set; }
    public int? ProfessionId { get; set; }
    public int? AddressId { get; set; }
    public Profession? Profession { get; set; }
    public Address? Address { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ApplicationUser()
    {
    }

    public static ApplicationUser Create(string username, string email,string fullName, string cin, DateTime birthDate, int countryId, int addressId, int professionId)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = email,
            FullName = fullName,
            Cin = cin,
            BirthDate = birthDate,
            CountryId = countryId,
            AddressId = addressId,
            ProfessionId = professionId,
            CreatedAt = DateTime.UtcNow,
            Status = 1,
            IsAccountConfirmed = false
        };
    }
}
