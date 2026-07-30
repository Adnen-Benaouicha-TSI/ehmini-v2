using Ehmini.Application.DTOs.Person;
using Ehmini.Application.Interfaces.PersonProviderService;
using Ehmini.Core.Enum;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Ehmini.Infrastructure.Providers.Pheonix
{
    public class PhoenixPersonProvider : IPersonProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProviderType Provider => ProviderType.Phoenix;

        public PhoenixPersonProvider(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PersonDto> GetPersonAsync(
        CancellationToken cancellationToken)
        {
            var currentToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();


            if (!string.IsNullOrEmpty(currentToken))
            {
                var pureToken = currentToken.StartsWith("Bearer ",
                    StringComparison.OrdinalIgnoreCase)
                    ? currentToken.Substring(7).Trim()
                    : currentToken.Trim();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        pureToken);
            }


            var response = await _httpClient.GetAsync(
                "api/Be/person",
                cancellationToken);


            var content = await response.Content.ReadAsStringAsync(
                cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Erreur Phoenix ({response.StatusCode}): {content}");
            }


            var person = JsonSerializer.Deserialize<PersonDto>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


            if (person == null || person.PersonId == 0)
            {
                throw new InvalidOperationException(
                    "Impossible de récupérer les informations du client depuis Phoenix.");
            }


            return person;
        }
    }
}
