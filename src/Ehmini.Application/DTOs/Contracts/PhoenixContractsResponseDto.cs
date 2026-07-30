

using Ehmini.Application.DTOs.Quotes;
using System.Text.Json.Serialization;

namespace Ehmini.Application.DTOs.Contracts
{

    public record PhoenixContractsResponseDto(
        [property: JsonPropertyName("count")]
    int Count,

        [property: JsonPropertyName("msg")]
    string Message,

        [property: JsonPropertyName("qs")]
    List<qModel> Contracts
    );
}
