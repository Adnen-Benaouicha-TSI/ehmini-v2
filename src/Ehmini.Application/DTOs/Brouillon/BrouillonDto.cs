using System.Text.Json.Serialization;

namespace Ehmini.Application.DTOs.Brouillon
{

    public record BrouillonDto(
        [property: JsonPropertyName("id")]
    int Id,

        [property: JsonPropertyName("parsedJson")]
    string? ParsedJson,

        [property: JsonPropertyName("description")]
    string? Description,

        [property: JsonPropertyName("userId")]
    int UserId,

        [property: JsonPropertyName("dateCreation")]
    DateTime DateCreation
    );
}
