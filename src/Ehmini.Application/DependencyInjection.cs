using Ehmini.Application.Interfaces;
using Ehmini.Application.Interfaces.PersonProviderService;
using Ehmini.Application.Interfaces.QuoteProvider;
using Ehmini.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ehmini.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IProviderConfigurationService, ProviderConfigurationService>();
        services.AddScoped<IQuoteProviderFactory, QuoteProviderFactory>();
        services.AddScoped<IDocumentOrchestrationService, DocumentOrchestrationService>();
        services.AddScoped<IPersonProviderFactory, PersonProviderFactory>();
        services.AddScoped<IProfessionService, ProfessionService>();

        return services;
    }
}