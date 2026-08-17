using Ehmini.Application.DTOs.Quotes;

namespace Ehmini.Application.Interfaces;

public interface IPhoenixTokenService
{
    Task<PhoenixTokenResponse> GetPhoenixTokenAsync(string cin, CancellationToken cancellationToken);
}