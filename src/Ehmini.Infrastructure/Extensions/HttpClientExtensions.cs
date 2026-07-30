using Microsoft.Extensions.DependencyInjection;
namespace Ehmini.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static IHttpClientBuilder AddPhoenixHttpClient<TClient, TImplementation>(
            this IServiceCollection services)
            where TClient : class
            where TImplementation : class, TClient
        {
            return services.AddHttpClient<TClient, TImplementation>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:2487/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });
        }
    }
}
