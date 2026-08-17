using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.DTOs.Person;
using Ehmini.Application.Interfaces;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using System.Security.Claims;

namespace Ehmini.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IPersonService _personService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICountryRepository _countryRepository;

    public class ValidationException : Exception { public ValidationException(string message) : base(message) { } }

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ITokenService tokenService, IEmailService emailService, IUnitOfWork unitOfWork, ICountryRepository countryRepository, IPersonService personService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _countryRepository = countryRepository;
        _roleManager = roleManager;
        _personService = personService;
    }

    public async Task<TokenResponseDto> RefreshAsync(TokenRequestDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.AccessToken) || string.IsNullOrEmpty(dto.RefreshToken))
        {
            throw new ArgumentException("Requête de jeton invalide.");
        }

        // 1. Extraire le Principal (les claims) depuis le token expiré
        var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
        var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            throw new UnauthorizedAccessException("Token d'accès invalide.");
        }

        // 2. Trouver l'utilisateur en BDD
        var user = await _userManager.FindByEmailAsync(userEmail);

        // 3. Valider le Refresh Token et sa date d'expiration
        if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh Token invalide ou expiré.");
        }

        // 4. Générer la nouvelle paire de jetons
        var newAccessToken = _tokenService.GenerateJwtToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // 5. Mettre à jour les informations en Base de Données
        user.RefreshToken = newRefreshToken;
        // On peut optionnellement repousser l'expiration du refresh token ici aussi
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Erreur lors de la mise à jour des jetons en base de données.");
        }

        return new TokenResponseDto(newAccessToken, newRefreshToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Identifiants incorrects.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Identifiants incorrects.");
        }

        var token = _tokenService.GenerateJwtToken(user);
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(15);

        // 2. ✨ AJOUT : Générer le Refresh Token cryptographique
        var refreshToken = _tokenService.GenerateRefreshToken();

        // 3. ✨ AJOUT : Enregistrer le Refresh Token sur l'entité de l'utilisateur
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Expire dans 7 jours

        await _userManager.UpdateAsync(user); // Sauvegarde en Base de Données

        return new AuthResponseDto(
            AccessToken: token,
            RefreshToken: refreshToken,
            Expiration: accessTokenExpiration,
            UserName: user.UserName ?? string.Empty,
            Email: user.Email ?? string.Empty
        );
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _userManager.Users.AnyAsync(u => u.Cin == dto.Cin))
        {
            return new RegisterResponseDto(false,"-3", "CIN Exists");
        }

        if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == dto.Phone))
        {
            return new RegisterResponseDto(false, "-1", "Phone Exists");
        }

        if (await _userManager.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return new RegisterResponseDto(false, "-2", "Email Exists");
        }
        int addressId = dto.AddressId;
        int professionId = dto.ProfessionId;
        if (dto.AddressId == 0)
        {
            addressId = 8;
        }
        if (dto.ProfessionId == 0)
        {
            professionId = 1;
        }
        var country = await _countryRepository.GetByIsoCodeAsync(dto.CountryId);
        if (country == null)
        {
            return new RegisterResponseDto(false, "-6", "La création de votre compte a échoué (Erreur de country not existe).");
        }

        var confirmationCode = new Random().Next(0, 1000000).ToString("D6");


        var user = ApplicationUser.Create(
            dto.Username,
            dto.Email,
            dto.FullName,
            dto.Cin,
            dto.BirthDate,
            1,
            8,
            1
        );

        user.PhoneNumber = dto.Phone;
        user.AccountConfirmationToken = confirmationCode;

        var identityResult = await _userManager.CreateAsync(user, dto.Password);
        if (!identityResult.Succeeded)
        {
            var firstError = identityResult.Errors.FirstOrDefault()?.Description ?? "Registration failed";
            return new RegisterResponseDto(false, "-5", firstError);
        }

        await _userManager.AddToRoleAsync(user, "ClientEhmini");

        var body = $"Votre code de confirmation de création de compte est {confirmationCode}";
        bool emailSent = await _emailService.SendEmailAsync(user.Email!, "Account confirmation token", body);

        if (emailSent)
        {
            return new RegisterResponseDto(true, user.Id.ToString(), "Un Email de confirmation d'inscription a été envoyé");
        }
        else
        {
            await _userManager.DeleteAsync(user);
            return new RegisterResponseDto(false, "-4", "La création de votre compte a échoué (Erreur d'envoi d'email).");
        }
    }


    public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto dto)
    {
        Log.Information($"--------------- ChangePassword pour {dto.Email} begin ---------------");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            Log.Warning($"ChangePassword : Utilisateur {dto.Email} introuvable.");
            return new ChangePasswordResponseDto(false, "La mise à jour du mot de passe a échoué, veuillez vérifier vos identifiants.");
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            Log.Information($"ChangePassword : L'utilisateur {dto.Email} a fourni un ancien mot de passe incorrect.");
            return new ChangePasswordResponseDto(false, "La mise à jour du mot de passe a échoué, veuillez vérifier votre ancien mot de passe.");
        }

        await _emailService.SendEmailAsync(user.Email!, "Sécurité Ehmini : Mot de passe modifié",
            "<p>Votre mot de passe vient d'être modifié avec succès. Si vous n'êtes pas à l'origine de cette action, contactez le support.</p>");

        Log.Information($"ChangePassword : Le mot de passe de {dto.Email} a été mis à jour avec succès.");
        Log.Information("--------------- ChangePassword end ---------------");

        return new ChangePasswordResponseDto(true, "Votre mot de passe a été mis à jour avec succès");
    }

    public async Task<RequestResetPasswordResponseDto> RequestResetPasswordAsync(RequestResetPasswordDto dto)
    {
        Log.Information("--------------- RequestResetPassword begin ---------------");

        var user = await _userManager.FindByEmailAsync(dto.Identifier)
                   ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Identifier);

        if (user == null)
        {
            Log.Information($"RequestResetPassword: L'utilisateur avec l'identifiant {dto.Identifier} n'existe pas");
            return new RequestResetPasswordResponseDto(-2, "L'utilisateur n'existe pas");
        }

        string resetCode = new Random().Next(0, 1000000).ToString("D6");

        user.AccountConfirmationToken = resetCode;
        user.PwdResetTokenCreationDate = DateTime.UtcNow;

        var body = $"Votre code de réinitialisation de mot de passe est {resetCode}.";
        bool emailSent = await _emailService.SendEmailAsync(user.Email!, "Password reset token", body);

        if (emailSent)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Log.Error("RequestResetPassword: Échec de la mise à jour de l'utilisateur dans Identity");
                return new RequestResetPasswordResponseDto(-1, "La demande de réinitialisation du mot de passe a échoué");
            }

            Log.Information($"RequestResetPassword: Remise à zéro envoyée par mail pour {user.Email}");
            Log.Information("--------------- RequestResetPassword end ---------------");
            return new RequestResetPasswordResponseDto(1, "Un Email de réinitialisation de mot de passe a été envoyé");
        }
        else
        {
            Log.Information("RequestResetPassword: L'envoi de l'email a échoué");
            return new RequestResetPasswordResponseDto(-1, "La demande de réinitialisation du mot de passe a échoué");
        }
    }

    public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        Log.Information("--------------- ResetPassword begin ---------------");

        var user = await _userManager.FindByEmailAsync(dto.Username)
                   ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Username);

        if (user == null)
        {
            Log.Information($"ResetPassword: user: {dto.Username} does not exist");
            return new ResetPasswordResponseDto(0, "L'utilisateur n'existe pas");
        }

        DateTime hourAgo = DateTime.UtcNow.AddHours(-1);

        if (user.AccountConfirmationToken == dto.Token && user.PwdResetTokenCreationDate >= hourAgo)
        {
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (isPasswordValid)
            {
                Log.Information($"ResetPassword: user {dto.Username} attempted to reuse the current password.");
                return new ResetPasswordResponseDto(0, "Le nouveau mot de passe doit être différent de l'ancien.");
            }

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Erreur lors de la mise à jour du mot de passe";
                Log.Error($"ResetPassword: Identity error for user {dto.Username} : {errorMsg}");
                return new ResetPasswordResponseDto(0, "Echec de réinitialisation de votre mot de passe");
            }

            user.AccountConfirmationToken = null;
            user.PwdResetTokenCreationDate = null;
            await _userManager.UpdateAsync(user);

            var body = $@"
           <p>Bonjour {user.FullName},</p>
           <p>Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.</p>
           <p>Si vous n'êtes pas à l'origine de cette action, veuillez contacter notre support immédiatement.</p>
           <p>Merci,</p>";

            await _emailService.SendEmailAsync(user.Email!, "Ehmini : Votre mot de passe a été réinitialisé avec succès", body);

            Log.Information($"ResetPassword: password reset successfully for user {dto.Username}");
            Log.Information("--------------- ResetPassword end ---------------");

            return new ResetPasswordResponseDto(1, "Votre mot de passe a été réinitialisé avec succès");
        }
        else
        {
            Log.Information($"ResetPassword: password reset failed for user {dto.Username} (Code invalide ou expiré)");
            Log.Information("--------------- ResetPassword end---------------");

            return new ResetPasswordResponseDto(0, "Echec de réinitialisation de votre mot de passe");
        }
    }
    public async Task<object> ConfirmAccountAsync(ConfirmAccountRequestDto dto)
    {
        Log.Information("--------------- ConfirmAccount begin ---------------");

        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Email == dto.Username || u.PhoneNumber == dto.Username || u.Cin == dto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            Log.Information($"ConfirmAccount : user {dto.Username} not found or invalid password");
            Log.Information("--------------- ConfirmAccount end ---------------");
            return new { id = -1, msg = "La confirmation de votre compte a échoué, veuillez vérifier vos informations ou contacter votre agence" };
        }

        bool isSignatureDefined = user.Signature != null;

        if (!user.IsAccountConfirmed)
        {
            Log.Information($"ConfirmAccount : user {dto.Username} is not yet confirmed");

            if (user.AccountConfirmationToken == dto.Code)
            {
                user.IsAccountConfirmed = true;
                user.EmailConfirmed = true;
                user.AccountConfirmationToken = null;

                await _userManager.UpdateAsync(user);

                Log.Information($"ConfirmAccount : user {dto.Username} confirm his account successfully");
                Log.Information("--------------- ConfirmAccount end ---------------");

                return new
                {
                    id = user.Id.ToString(),
                    msg = "Votre compte a été confirmé avec succès",
                    isSignatureDefined = isSignatureDefined,
                    token = _tokenService.GenerateJwtToken(user)
                };
            }
            else
            {
                Log.Information($"ConfirmAccount : user {dto.Username} Invalid account confirmation attempt");
                Log.Information("--------------- ConfirmAccount end ---------------");
                return new { id = -1, msg = "La confirmation de votre compte a échoué, veuillez vérifier vos informations ou contacter votre agence" };
            }
        }
        else
        {
            Log.Information($"ConfirmAccount : user {dto.Username} already confirmed");
            Log.Information("--------------- ConfirmAccount end ---------------");
            return new
            {
                id = "0",
                isSignatureDefined = isSignatureDefined,
                msg = "Votre compte a déjà été vérifié"
            };
        }
    }

    public async Task<object> GetConfirmationCodeAsync(GetConfirmationCodeRequestDto dto)
    {
        Log.Information("--------------- GetConfirmationCode begin ---------------");

        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Email == dto.Username || u.PhoneNumber == dto.Username || u.Cin == dto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            Log.Information($"GetConfirmationCode : user {dto.Username} not found or invalid password");
            Log.Information("--------------- GetConfirmationCode end ---------------");
            return new { id = -1, msg = "L'envoi de l'e-mail de confirmation a échoué. Veuillez réessayer plus tard, ou contactez votre agence si le problème persiste." };
        }

        if (!user.IsAccountConfirmed)
        {
            string confirmationCode = new Random().Next(0, 1000000).ToString("D6");
            user.AccountConfirmationToken = confirmationCode;

            var body = $"Votre code de confirmation de création de compte est {confirmationCode} ";
            bool emailSent = await _emailService.SendEmailAsync(user.Email!, "Resend account confirmation token", body);

            if (emailSent)
            {
                await _userManager.UpdateAsync(user);
                Log.Information("GetConfirmationCode : Un email de confirmation d'inscription a été envoyé");
                Log.Information("--------------- GetConfirmationCode end ---------------");
                return new { id = user.Id.ToString(), msg = "Un email de confirmation d'inscription a été envoyé" };
            }
            else
            {
                Log.Information("GetConfirmationCode : L'envoi de l'e-mail de confirmation a échoué.");
                Log.Information("--------------- GetConfirmationCode end ---------------");
                return new { id = -1, msg = "L'envoi de l'e-mail de confirmation a échoué. Veuillez réessayer plus tard, ou contactez votre agence si le problème persiste." };
            }
        }
        else
        {
            Log.Information($"GetConfirmationCode : user {dto.Username} already confirmed");
            Log.Information("--------------- GetConfirmationCode end ---------------");
            return new { id = "0", msg = "Votre compte a déjà été vérifié" };
        }
    }

    public async Task<UpdateUserInfoResponseDto> UpdateUserInfoAsync(UpdateUserInfoRequestDto dto)
    {
        return await _personService.UpdateProfileAsync(dto, CancellationToken.None);
    }

    public async Task<PersonDto> GetPersonAsync(CancellationToken cancellationToken)
    {
        return await _personService.GetPersonAsync(cancellationToken);
    }
}