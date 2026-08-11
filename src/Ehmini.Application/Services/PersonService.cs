using Ehmini.Application.DTOs;
using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.DTOs.Person;
using Ehmini.Application.Interfaces;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace Ehmini.Application.Services;

public class PersonService : IPersonService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICountryRepository _countryRepository;
    private readonly IPhoenixPersonSyncService _phoenixSyncService;

    public PersonService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ICountryRepository countryRepository,
        IPhoenixPersonSyncService phoenixSyncService)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _countryRepository = countryRepository;
        _phoenixSyncService = phoenixSyncService;
    }

    public async Task<PersonDto> GetPersonAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users
            .Include(u => u.Profession)
            .Include(u => u.Address)
                .ThenInclude(a => a!.Locality)
                    .ThenInclude(l => l.Zone)
                        .ThenInclude(z => z.Region)
                            .ThenInclude(r => r.Country)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Impossible de récupérer les informations de l'utilisateur connecté.");
        }

        return MapToPersonDto(user);
    }

    public async Task<UpdateUserInfoResponseDto> UpdateProfileAsync(
        UpdateUserInfoRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            Log.Information("--------------- PersonService.UpdateProfileAsync begin ---------------");

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Cin == dto.Cin, cancellationToken);

            if (user == null)
            {
                Log.Warning($"UpdateProfileAsync : Impossible de trouver l'utilisateur avec le CIN {dto.Cin}");
                return new UpdateUserInfoResponseDto(false, $"Impossible de trouver l'utilisateur avec le numéro CIN : {dto.Cin}");
            }

            // --- Update local Ehmini DB ---
            if (!string.IsNullOrEmpty(dto.CountryIsoCode))
            {
                var country = await _countryRepository.GetByIsoCodeAsync(dto.CountryIsoCode);
                if (country != null)
                {
                    user.CountryId = country.Id;
                }
            }

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Phone != null) user.PhoneNumber = dto.Phone;
            if (dto.Signature != null) user.Signature = dto.Signature;
            if (dto.Birthday != null) user.BirthDate = dto.Birthday.Value;
            if (dto.ProfessionId > 0) user.ProfessionId = dto.ProfessionId;
            if (dto.AddressId > 0) user.AddressId = dto.AddressId;

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                string errorList = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                Log.Error($"UpdateProfileAsync Identity Error: {errorList}");
                return new UpdateUserInfoResponseDto(false, "Échec de la mise à jour des données dans AspNetUsers.");
            }

            Log.Information($"UpdateProfileAsync : Utilisateur avec CIN {dto.Cin} mis à jour localement avec succès");

            // --- Sync update to Phoenix (fire and log, don't fail the request) ---
            try
            {
                var phoenixResult = await _phoenixSyncService.SyncUpdateAsync(dto, cancellationToken);
                if (phoenixResult)
                {
                    Log.Information("UpdateProfileAsync : Synchronisation Phoenix réussie");
                }
                else
                {
                    Log.Warning("UpdateProfileAsync : Synchronisation Phoenix a échoué (réponse négative)");
                }
            }
            catch (Exception phoenixEx)
            {
                Log.Warning(phoenixEx, "UpdateProfileAsync : Erreur lors de la synchronisation Phoenix — mise à jour locale réussie");
            }

            Log.Information("--------------- PersonService.UpdateProfileAsync end ---------------");
            return new UpdateUserInfoResponseDto(true, "Les informations de l'utilisateur ont été mises à jour avec succès");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"UpdateProfileAsync Error : {ex.Message}");
            return new UpdateUserInfoResponseDto(false, ex.Message);
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Impossible d'identifier l'utilisateur connecté.");
        }

        return userId;
    }

    private static PersonDto MapToPersonDto(ApplicationUser user)
    {
        var address = user.Address;
        var locality = address?.Locality;
        var zone = locality?.Zone;
        var region = zone?.Region;
        var country = region?.Country;

        // Split FullName into first/last for compatibility with PersonDto
        var nameParts = (user.FullName ?? string.Empty).Split(' ', 2);
        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        return new PersonDto(
            PersonId: 0,
            CustomerUserId: 0,
            ClientId: 0,
            Cin: user.Cin,
            FirstName: firstName,
            LastName: lastName,
            FullName: user.FullName,
            BirthDate: user.BirthDate,
            Email: user.Email,
            Phone: user.PhoneNumber,
            Sex: null,
            AddressId: user.AddressId,
            Region: region?.Title,
            RegionId: region?.Id,
            Zone: zone?.Title,
            ZoneId: zone?.Id,
            Locality: locality?.Title,
            LocalityId: locality?.Id,
            Adresse: address?.Title,
            ZipCode: locality?.ZipCode,
            Profession: user.Profession != null
                ? new ProfessionDto(user.Profession.Id, user.Profession.Title)
                : null,
            Country: country != null
                ? new CountryDto(country.IsoCode ?? country.Id.ToString(), country.Title)
                : null,
            Signature: user.Signature,
            Message: null
        );
    }
}
