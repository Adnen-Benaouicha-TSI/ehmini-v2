using Ehmini.Application.DTOs.Auth;

namespace Ehmini.Application.Interfaces;

public interface IPhoenixPersonSyncService
{
    Task<bool> SyncUpdateAsync(UpdateUserInfoRequestDto dto, CancellationToken cancellationToken);
}
