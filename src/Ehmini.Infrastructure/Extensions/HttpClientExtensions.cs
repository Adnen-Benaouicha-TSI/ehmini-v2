using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ehmini.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static IHttpClientBuilder AddPhoenixHttpClient<TClient, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TClient : class
            where TImplementation : class, TClient
        {
            var baseUrl = configuration["Phoenix:BaseUrl"];
            var timeoutSeconds = configuration.GetValue<int>("Phoenix:TimeoutSeconds");
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Phoenix base URL is not configured.", nameof(baseUrl));
            }

            return services
          .AddHttpClient<TClient, TImplementation>(client =>
          {
              client.BaseAddress = new Uri(baseUrl);
              client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
          })
          .ConfigurePrimaryHttpMessageHandler(() =>
          {
              return new HttpClientHandler
              {
                  ServerCertificateCustomValidationCallback =
                      HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
              };
          });
        }
    }
}