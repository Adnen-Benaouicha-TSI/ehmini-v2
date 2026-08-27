using Ehmini.Application;
using Ehmini.Infrastructure;
using Ehmini.WebApi.Middleware;
using Ehmini.WebApi.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Enregistrement des Couches (Clean Architecture)
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration); // Contient déjà la DB, Identity et l'authentification JWT
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==========================================
// 2. Configuration de Swagger (JWT + API KEY)
// ==========================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ehmini API", Version = "v1" });

    // Configuration pour OAuth2 / JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Configuration pour l'API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Clé d'API requise pour les requêtes externes. Saisissez votre clé dans le champ ci-dessous.",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================================
// 3. Configuration CORS pour l'application Mobile Ionic
// ==========================================
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("AllowedHosts:AllowedOrigins")
        .Get<string[]>() ?? [];

    options.AddPolicy("IonicCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// ==========================================
// 4. Pipeline des Middlewares (Ordre Critique)
// ==========================================

// Gestion globale des exceptions
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Activation de CORS avant l'authentification
app.UseCors("IonicCorsPolicy");

// 💡 Si vous souhaitez activer le Middleware de clé d'API globalement, décommentez la ligne suivante :
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication(); // Validation OAuth2 / JWT
app.UseAuthorization();

app.MapControllers();
app.Run();