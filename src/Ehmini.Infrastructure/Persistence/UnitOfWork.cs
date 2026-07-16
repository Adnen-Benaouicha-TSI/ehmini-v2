using System;
using System.Threading;
using System.Threading.Tasks;
using Ehmini.Core.Interfaces;
using Ehmini.Infrastructure.Persistence.Repositories;

namespace Ehmini.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDocumentRepository? _documentRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }


    public IDocumentRepository Documents => _documentRepository ??= new DocumentRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
