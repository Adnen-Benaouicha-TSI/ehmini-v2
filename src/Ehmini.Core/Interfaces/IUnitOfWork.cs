using System.Threading;
using System.Threading.Tasks;

namespace Ehmini.Core.Interfaces;

public interface IUnitOfWork
{
    IDocumentRepository Documents { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
