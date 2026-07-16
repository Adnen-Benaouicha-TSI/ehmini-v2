using Ehmini.Application.DTOs.Quotes;
using Ehmini.Application.Interfaces;
using Ehmini.Core.Enum;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Ehmini.Infrastructure.Providers;

public class PhoenixQuoteProvider : IQuoteProvider
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ProviderType Provider => ProviderType.Phoenix;

    public PhoenixQuoteProvider(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<ProviderQuoteResponseDto> GenerateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    {
        var currentToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(currentToken))
        {
            string pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? currentToken.Substring(7).Trim()
                : currentToken.Trim();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pureToken);
        }


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
            Description: "Devis Phoenix",
            Quantite: 1,
            UnitPrice: apiResult.qModel.ttc,
            LineTotal: apiResult.qModel.ttc
        )
    };

        return new ProviderQuoteResponseDto(
            Reference: apiResult.qModel.reference,
            TotalAmount: apiResult.qModel.ttc,
            Lines: lines,
            ExpiresAt: DateTime.UtcNow.AddDays(30)
        );
    }

    public async Task<ProviderQuoteResponseDto> UpdateQuoteAsync(qModel phoenixPayload, CancellationToken cancellationToken)
    {
        var currentToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(currentToken))
        {
            string pureToken = currentToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? currentToken.Substring(7).Trim()
                : currentToken.Trim();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pureToken);
        }

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
            Description: "Devis Phoenix",
            Quantite: 1,
            UnitPrice: apiResult.qModel.ttc,
            LineTotal: apiResult.qModel.ttc
        )
    };

        return new ProviderQuoteResponseDto(
            Reference: apiResult.qModel.reference,
            TotalAmount: apiResult.qModel.ttc,
            Lines: lines,
            ExpiresAt: DateTime.UtcNow.AddDays(30)
        );
    }

}
