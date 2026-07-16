using Ehmini.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ehmini.Infrastructure.Services;

public class ApiKeyValidation : IApiKeyValidation
{
    private readonly IConfiguration _configuration;
    private const string ApiKeySectionName = "Authentication:ApiKey";

    public ApiKeyValidation(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsValidApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return false;

        var clientApiKey = _configuration[ApiKeySectionName];
        return clientApiKey == apiKey;
    }
}