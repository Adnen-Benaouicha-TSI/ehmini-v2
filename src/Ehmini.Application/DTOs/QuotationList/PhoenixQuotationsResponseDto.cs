using System.Text.Json.Serialization;


namespace Ehmini.Application.DTOs.QuotationList
{

    public record PhoenixQuotationsResponseDto(
        [property: JsonPropertyName("isSucceeded")]
    bool IsSucceeded,

        [property: JsonPropertyName("msg")]
    string? Message,

        [property: JsonPropertyName("data")]
    List<ProviderQuotationDto> Data
    );
}
