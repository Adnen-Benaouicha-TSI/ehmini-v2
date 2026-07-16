using System.Threading.Tasks;
using Ehmini.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ehmini.WebApi.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyValidation apiKeyValidation)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("La clé d'API est manquante.");
            return;
        }

        if (!apiKeyValidation.IsValidApiKey(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Clé d'API invalide.");
            return;
        }

        await _next(context);
    }
}