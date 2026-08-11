namespace Ehmini.Application.Interfaces;

public interface IPhoenixTokenService
{
    Task<string> GetPhoenixTokenAsync(string cin, CancellationToken cancellationToken);
}