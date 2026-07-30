using System.Text.Json.Serialization;


namespace Ehmini.Application.DTOs.Brouillon
{

    public record PhoenixBrouillonsResponseDto(
        [property: JsonPropertyName("isSucceeded")]
    bool IsSucceeded,

        [property: JsonPropertyName("msg")]
    string? Message,

        [property: JsonPropertyName("data")]
    List<BrouillonDto> Data
    );
}
