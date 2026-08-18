using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.Interfaces;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Ehmini.Infrastructure.Providers.Pheonix;

public class PhoenixPersonSyncService : IPhoenixPersonSyncService
{
    private readonly HttpClient _httpClient;

    public PhoenixPersonSyncService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SyncUpdateAsync(UpdateUserInfoRequestDto dto, CancellationToken cancellationToken)
    {
        Log.Information("PhoenixPersonSyncService : Envoi de la mise à jour vers Phoenix...");

        var payload = new
        {
            Cin = dto.Cin,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            CountryIsoCode = dto.CountryIsoCode,
            Birthday = dto.Birthday,
            ProfessionId = dto.ProfessionId,
            Signature = dto.Signature
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            "api/Auth/updateUser",
            jsonContent,
            cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            Log.Warning($"PhoenixPersonSyncService : Phoenix a répondu avec {response.StatusCode} — {responseContent}");
            return false;
        }

        // Parse Phoenix response to check isSucceeded
        try
        {
            using var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("isSucceeded", out var succeededProp))
            {
                return succeededProp.GetBoolean();
            }
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "PhoenixPersonSyncService : Impossible de parser la réponse Phoenix");
        }

        return true;
    }
}
