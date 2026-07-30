using System.Text.Json.Serialization;


namespace Ehmini.Application.DTOs.QuotationList
{

    public record PaymentSplittingDto(
        [property: JsonPropertyName("id")]
    int Id,

        [property: JsonPropertyName("active")]
    bool Active,

        [property: JsonPropertyName("dDate")]
    string DueDate,

        [property: JsonPropertyName("dateEnd")]
    string EndDate,

        [property: JsonPropertyName("total")]
    decimal Total
    );
}
