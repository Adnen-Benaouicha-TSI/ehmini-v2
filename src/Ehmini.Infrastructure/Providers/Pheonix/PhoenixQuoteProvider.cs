using Ehmini.Application.DTOs.Brouillon;
using Ehmini.Application.DTOs.Contracts;
using Ehmini.Application.DTOs.QuotationList;
using Ehmini.Application.DTOs.Quotes;
using Ehmini.Application.Interfaces;
using Ehmini.Application.Interfaces.QuoteProvider;
using Ehmini.Core.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Ehmini.Infrastructure.Providers.Pheonix;

public class PhoenixQuoteProvider : IQuoteProvider, IPhoenixTokenService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    public ProviderType Provider => ProviderType.Phoenix;

    public PhoenixQuoteProvider(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    //public async Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    //{
    //    // ==========================================
    //    // 1. SÉCURITÉ ANTI-BOUCLE (À faire en premier !)
    //    // ==========================================
    //    // On rompt la référence circulaire pour éviter que le sérialiseur JSON ne boucle
    //    if (phoenixPayload?.c != null)
    //    {
    //        phoenixPayload.c.quotation = null!;
    //    }

    //    // ==========================================
    //    // 2. PRÉPARATION DE L'UNIQUE REQUÊTE HTTP
    //    // ==========================================
    //    var request = new HttpRequestMessage(HttpMethod.Post, "api/Be/setQuotation")
    //    {
    //        // On utilise le payload déjà nettoyé
    //        Content = JsonContent.Create(phoenixPayload)
    //    };

    //    // ==========================================
    //    // 3. INJECTION DU TOKEN JWT
    //    // ==========================================
    //    var currentToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

    //    if (!string.IsNullOrEmpty(currentToken))
    //    {
    //        string pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
    //            ? currentToken.Substring(7).Trim()
    //            : currentToken.Trim();

    //        // On applique le token sur l'unique requête
    //        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pureToken);
    //    }

    //    // ==========================================
    //    // 4. ENVOI UNIQUE ET SÉCURISÉ
    //    // ==========================================
    //    var response = await _httpClient.SendAsync(request, cancellationToken);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
    //        throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
    //    }

    //    // ==========================================
    //    // 5. DÉSÉRIALISATION DE LA RÉPONSE
    //    // ==========================================
    //    var apiResult = await response.Content.ReadFromJsonAsync<PhoenixApiResponse>(cancellationToken: cancellationToken);

    //    if (apiResult == null || apiResult.msg != "OK")
    //    {
    //        throw new InvalidOperationException($"L'API Phoenix a retourné une erreur lors du calcul : {apiResult?.track ?? "Inconnue"}");
    //    }

    //    // ==========================================
    //    // 6. MAPPING DES LIGNES
    //    // ==========================================
    //    var lines = new List<QuoteLineDto>
    //{
    //    new QuoteLineDto(
    //        CoverageCode: "RC",
    //        Label: "Responsabilité Civile",
    //        Premium: apiResult.qModel.net,
    //        Quantite: 1
    //    )
    //};

    //    return new ProviderQuoteResponseDto(
    //        ExternalReference: apiResult.qModel.reference,
    //        GrossPremium: apiResult.qModel.ttc,
    //        NetPremium: apiResult.qModel.net,
    //        Quantite: 1,
    //        Lines: lines,
    //        ExpiresAt: DateTime.UtcNow.AddDays(30)
    //    );
    //}
    public async Task<PhoenixTokenResponse> GetPhoenixTokenAsync(
        string cin,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            grant_type = "client_credentials",
            client_id = _configuration["Phoenix:ClientId"],
            client_secret = _configuration["Phoenix:ClientSecret"],
            user_cin = cin
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/oauth/token",
            payload,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Échec d'obtention du token Phoenix: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<PhoenixTokenResponse>(
            cancellationToken: cancellationToken);

        return result;
    }
    public async Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Extraire le cin depuis le token utilisateur d'Ehmini (déjà authentifié côté Ehmini)
            var currentPrincipal = _httpContextAccessor.HttpContext?.User;
            var cin = currentPrincipal?.FindFirst("cin")?.Value;

            if (string.IsNullOrEmpty(cin))
            {
                throw new UnauthorizedAccessException("cin introuvable dans le token utilisateur Ehmini.");
            }

            // 2. Échanger contre un token Phoenix (via client_credentials + cin)
            string phoenixToken = (await GetPhoenixTokenAsync(cin, cancellationToken)).access_token;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);

            if (phoenixPayload.c != null)
            {
                phoenixPayload.c.quotation = null!;
            }

            var response = await _httpClient.PostAsJsonAsync("api/Be/setQuotation", phoenixPayload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
            }

            var apiResult = await response.Content.ReadFromJsonAsync<PhoenixApiResponse>(cancellationToken: cancellationToken);

            if (apiResult == null || apiResult.msg != "OK")
            {
                throw new InvalidOperationException($"L'API Phoenix a retourné une erreur lors du calcul : {apiResult?.track ?? "Inconnue"}");
            }

            var lines = new List<QuoteLineDto>
        {
            new QuoteLineDto(
                Description: "Devis",
                Quantite: 1,
                UnitPrice: apiResult.qModel.ttc ?? 0,
                LineTotal: apiResult.qModel.ttc ?? 0
            )
        };

            return new ProviderQuoteResponseDto(
                Reference: apiResult.qModel.reference,
                TotalAmount: apiResult.qModel.ttc ?? 0,
                Lines: lines,
                ExpiresAt: DateTime.UtcNow.AddDays(30),
                reponsePheonix: apiResult
            );
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Interception du Timeout HTTP de 60s
            throw new TimeoutException("L'API Phoenix n'a pas répondu dans le délai imparti.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors de la génération du devis : {ex.Message}", ex);
        }
    }

    //public async Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    //{
    //    // 1. Extraire le cin depuis le token utilisateur d'Ehmini (déjà authentifié côté Ehmini)
    //    var currentPrincipal = _httpContextAccessor.HttpContext?.User;
    //    var cin = currentPrincipal?.FindFirst("cin")?.Value;

    //    if (string.IsNullOrEmpty(cin))
    //    {
    //        throw new UnauthorizedAccessException("cin introuvable dans le token utilisateur Ehmini.");
    //    }

    //    // 2. Échanger contre un token Phoenix (via client_credentials + cin)
    //    string phoenixToken = await GetPhoenixTokenAsync(cin, cancellationToken);

    //    _httpClient.DefaultRequestHeaders.Authorization =
    //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);

    //    if (phoenixPayload.c != null)
    //    {
    //        phoenixPayload.c.quotation = null!;
    //    }

    //    _httpClient.Timeout = TimeSpan.FromSeconds(60);
    //    var response = await _httpClient.PostAsJsonAsync("api/Be/setQuotation", phoenixPayload, cancellationToken);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
    //        throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
    //    }

    //    var apiResult = await response.Content.ReadFromJsonAsync<PhoenixApiResponse>(cancellationToken: cancellationToken);

    //    if (apiResult == null || apiResult.msg != "OK")
    //    {
    //        throw new InvalidOperationException($"L'API Phoenix a retourné une erreur lors du calcul : {apiResult?.track ?? "Inconnue"}");
    //    }

    //    var lines = new List<QuoteLineDto>
    //{
    //    new QuoteLineDto(
    //        Description: "Devis",
    //        Quantite: 1,
    //        UnitPrice: apiResult.qModel.ttc,
    //        LineTotal: apiResult.qModel.ttc
    //    )
    //};

    //    return new ProviderQuoteResponseDto(
    //        Reference: apiResult.qModel.reference,
    //        TotalAmount: apiResult.qModel.ttc,
    //        Lines: lines,
    //        ExpiresAt: DateTime.UtcNow.AddDays(30)
    //    );
    //}


    //public async Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    //{
    //    var currentToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

    //    if (!string.IsNullOrEmpty(currentToken))
    //    {
    //        string pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
    //            ? currentToken.Substring(7).Trim()
    //            : currentToken.Trim();

    //        _httpClient.DefaultRequestHeaders.Authorization =
    //            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pureToken);
    //    }


    //    if (phoenixPayload.c != null)
    //    {
    //        phoenixPayload.c.quotation = null!;
    //    }
    //    _httpClient.Timeout = TimeSpan.FromSeconds(60);
    //    var response = await _httpClient.PostAsJsonAsync("api/Be/setQuotation", phoenixPayload, cancellationToken);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
    //        throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
    //    }

    //    var apiResult = await response.Content.ReadFromJsonAsync<PhoenixApiResponse>(cancellationToken: cancellationToken);

    //    if (apiResult == null || apiResult.msg != "OK")
    //    {
    //        throw new InvalidOperationException($"L'API Phoenix a retourné une erreur lors du calcul : {apiResult?.track ?? "Inconnue"}");
    //    }

    //    var lines = new List<QuoteLineDto>
    //{
    //    new QuoteLineDto(
    //        Description: "Devis Phoenix",
    //        Quantite: 1,
    //        UnitPrice: apiResult.qModel.ttc,
    //        LineTotal: apiResult.qModel.ttc
    //    )
    //};

    //    return new ProviderQuoteResponseDto(
    //        Reference: apiResult.qModel.reference,
    //        TotalAmount: apiResult.qModel.ttc,
    //        Lines: lines,
    //        ExpiresAt: DateTime.UtcNow.AddDays(30)
    //    );
    //}

    public async Task<ProviderQuoteResponseDto> UpdateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    {
        var currentPrincipal = _httpContextAccessor.HttpContext?.User;
        var cin = currentPrincipal?.FindFirst("cin")?.Value;

        if (string.IsNullOrEmpty(cin))
        {
            throw new UnauthorizedAccessException("cin introuvable dans le token utilisateur Ehmini.");
        }

        // 2. Échanger contre un token Phoenix (via client_credentials + cin)
        string phoenixToken = (await GetPhoenixTokenAsync(cin, cancellationToken)).access_token;

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);

        if (phoenixPayload.c != null)
        {
            phoenixPayload.c.quotation = null!;
        }

        var response = await _httpClient.PostAsJsonAsync("api/Be/updateQuotation", phoenixPayload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
        }

        var apiResult = await response.Content.ReadFromJsonAsync<PhoenixApiResponse>(cancellationToken: cancellationToken);

        if (apiResult == null || apiResult.msg != "OK")
        {
            throw new InvalidOperationException($"L'API Phoenix a retourné une erreur lors du calcul : {apiResult?.track ?? "Inconnue"}");
        }

        var lines = new List<QuoteLineDto>
    {
        new QuoteLineDto(
            Description: "Devis",
            Quantite: 1,
            UnitPrice: apiResult.qModel.ttc ?? 0,
            LineTotal: apiResult.qModel.ttc ?? 0
        )
    };

        return new ProviderQuoteResponseDto(
            Reference: apiResult.qModel.reference,
            TotalAmount: apiResult.qModel.ttc ?? 0,
            Lines: lines,
            ExpiresAt: DateTime.UtcNow.AddDays(30),
            reponsePheonix: apiResult
        );
    }

    public async Task<List<ProviderQuotationDto>> GetQuotationsAsync(int language, CancellationToken cancellationToken)
    {
        var currentToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        using var timeoutCts = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );
        if (!string.IsNullOrEmpty(currentToken))
        {
            string pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? currentToken.Substring(7).Trim()
                : currentToken.Trim();

            var currentPrincipal = _httpContextAccessor.HttpContext?.User;
            var cin = currentPrincipal?.FindFirst("cin")?.Value;

            string phoenixToken = (await GetPhoenixTokenAsync(cin, timeoutCts.Token)).access_token;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);
        }


        var response = await _httpClient.GetAsync(
            $"api/Be/getQuotations?language={language}",
            timeoutCts.Token);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Erreur Phoenix ({response.StatusCode}): {errorContent}");
        }

        var apiResult = await response.Content.ReadFromJsonAsync<PhoenixQuotationsResponseDto>(
            cancellationToken: cancellationToken);

        if (apiResult == null)
        {
            throw new InvalidOperationException("La réponse de Phoenix est vide.");
        }

        if (!apiResult.IsSucceeded)
        {
            throw new InvalidOperationException(
                $"L'API Phoenix a retourné une erreur : {apiResult.Message ?? "Inconnue"}");
        }

        return apiResult.Data;
    }
    public async Task<List<qModel>> GetContractsAsync(int language, CancellationToken cancellationToken)
    {
        var currentToken = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString();

        using var timeoutCts = new CancellationTokenSource(
         TimeSpan.FromSeconds(60)
     );

        if (!string.IsNullOrEmpty(currentToken))
        {
            string pureToken = currentToken.StartsWith(
                "Bearer ",
                StringComparison.OrdinalIgnoreCase)
                ? currentToken.Substring(7).Trim()
                : currentToken.Trim();
            var currentPrincipal = _httpContextAccessor.HttpContext?.User;
            var cin = currentPrincipal?.FindFirst("cin")?.Value;

            string phoenixToken = (await GetPhoenixTokenAsync(cin, timeoutCts.Token)).access_token;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);
        }



        var response = await _httpClient.GetAsync(
            $"api/Be/contracts?language={language}",
            timeoutCts.Token);


        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Erreur Phoenix ({response.StatusCode}): {errorContent}");
        }


        var apiResult = await response.Content
            .ReadFromJsonAsync<PhoenixContractsResponseDto>(
                cancellationToken: cancellationToken);


        if (apiResult == null)
        {
            throw new InvalidOperationException(
                "La réponse Phoenix est vide.");
        }


        if (apiResult.Message != "OK")
        {
            throw new InvalidOperationException(
                "L'API Phoenix a retourné une erreur lors de la récupération des contrats.");
        }


        return apiResult.Contracts;
    }
    public async Task<List<BrouillonDto>> GetBrouillonsByUserAsync(
    CancellationToken cancellationToken)
    {
        var currentToken = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString();
        using var timeoutCts = new CancellationTokenSource(
    TimeSpan.FromSeconds(60)
);
        if (!string.IsNullOrEmpty(currentToken))
        {
            var pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? currentToken.Substring(7).Trim()
                : currentToken.Trim();
            var currentPrincipal = _httpContextAccessor.HttpContext?.User;
            var cin = currentPrincipal?.FindFirst("cin")?.Value;

            string phoenixToken = (await GetPhoenixTokenAsync(cin, timeoutCts.Token)).access_token;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", phoenixToken);
        }

        var response = await _httpClient.GetAsync(
            "api/Be/GetBrouillonsByUser",
            timeoutCts.Token);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Erreur Phoenix ({response.StatusCode}): {errorContent}");
        }

        var apiResult = await response.Content.ReadFromJsonAsync<PhoenixBrouillonsResponseDto>(
            cancellationToken: cancellationToken);

        if (apiResult == null)
        {
            throw new InvalidOperationException(
                "La réponse Phoenix est vide.");
        }

        if (!apiResult.IsSucceeded)
        {
            throw new InvalidOperationException(
                apiResult.Message ?? "Erreur lors de la récupération des brouillons.");
        }

        return apiResult.Data;
    }
}
