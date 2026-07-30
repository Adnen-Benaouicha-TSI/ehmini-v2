using Ehmini.Application.Interfaces;
using Ehmini.Application.Interfaces.PersonProviderService;
using Ehmini.Application.Interfaces.QuoteProvider;
using Ehmini.Application.Services;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Ehmini.Infrastructure.Extensions;
using Ehmini.Infrastructure.Persistence;
using Ehmini.Infrastructure.Persistence.Repositories;
using Ehmini.Infrastructure.Providers.Pheonix;
using Ehmini.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // 👈 AJOUTÉ
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens; // 👈 AJOUTÉ
using System.Text;

namespace Ehmini.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection();

        // 1. Base de données
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. Identity Configuration
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // 3. ✨ CONFIGURATION AUTHENTIFICATION ET SCHÉMAS PAR DÉFAUT (RÉSOLUT L'ERREUR 500)
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret absent de la configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        // 4. Enregistrement des Services et Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IDocumentDetailRepository, DocumentDetailRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<ILocalityRepository, LocalityRepository>();
        services.AddScoped<IProfessionRepository, ProfessionRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();
        services.AddPhoenixHttpClient<IQuoteProvider, PhoenixQuoteProvider>();
        services.AddPhoenixHttpClient<IPersonProvider, PhoenixPersonProvider>();

        return services;
    }
}