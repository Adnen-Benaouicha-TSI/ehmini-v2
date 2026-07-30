
namespace Ehmini.Application.DTOs.QuotationList
{
    using System.Text.Json.Serialization;

    public record ProviderQuotationDto(
        [property: JsonPropertyName("id")]
    int Id,

        [property: JsonPropertyName("refrence")]
    string Reference,

        [property: JsonPropertyName("creationDate")]
    DateTime CreationDate,

        [property: JsonPropertyName("clientId")]
    int ClientId,

        [property: JsonPropertyName("totalTTC")]
    decimal TotalTTC,

        [property: JsonPropertyName("branchId")]
    int? BranchId,

        [property: JsonPropertyName("branchTitle")]
    string? BranchTitle,

        [property: JsonPropertyName("branchTitleAr")]
    string? BranchTitleAr,

        [property: JsonPropertyName("branchTitleEn")]
    string? BranchTitleEn,

        [property: JsonPropertyName("productTitle")]
    string? ProductTitle,

        [property: JsonPropertyName("productTitleAr")]
    string? ProductTitleAr,

        [property: JsonPropertyName("productTitleEn")]
    string? ProductTitleEn,

        [property: JsonPropertyName("paymentSplitting")]
    IReadOnlyList<PaymentSplittingDto> PaymentSplitting
    );
}
