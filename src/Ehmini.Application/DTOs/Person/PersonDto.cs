
using System.Text.Json.Serialization;

namespace Ehmini.Application.DTOs.Person
{

    public record PersonDto(

        [property: JsonPropertyName("personId")]
    int PersonId,

        [property: JsonPropertyName("customerUserId")]
    int CustomerUserId,

        [property: JsonPropertyName("clientId")]
    int ClientId,

        [property: JsonPropertyName("cin")]
    string Cin,

        [property: JsonPropertyName("firstName")]
    string FirstName,

        [property: JsonPropertyName("lastName")]
    string LastName,

        [property: JsonPropertyName("fullName")]
    string FullName,

        [property: JsonPropertyName("birthDate")]
    DateTime? BirthDate,

        [property: JsonPropertyName("email")]
    string? Email,

        [property: JsonPropertyName("phone")]
    string? Phone,

        [property: JsonPropertyName("sex")]
    string? Sex,

        [property: JsonPropertyName("ad")]
    int? AddressId,

        [property: JsonPropertyName("region")]
    string? Region,

        [property: JsonPropertyName("regionId")]
    int? RegionId,

        [property: JsonPropertyName("zone")]
    string? Zone,

        [property: JsonPropertyName("zoneId")]
    int? ZoneId,

        [property: JsonPropertyName("locality")]
    string? Locality,

        [property: JsonPropertyName("localityId")]
    int? LocalityId,

        [property: JsonPropertyName("adresse")]
    string? Adresse,

        [property: JsonPropertyName("zipCode")]
    int? ZipCode,

        [property: JsonPropertyName("profession")]
    ProfessionDto? Profession,

        [property: JsonPropertyName("country")]
    CountryDto? Country,

        [property: JsonPropertyName("signature")]
    string? Signature,

        [property: JsonPropertyName("msg")]
    string? Message
    );

}
