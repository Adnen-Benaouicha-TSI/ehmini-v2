using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(d => d.Details)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Document?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(d => d.Details)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    public async Task<Document?> GetWithReferenceAsync(string reference)
    {
        return await _dbSet
            .Include(d => d.Details)
            .FirstOrDefaultAsync(d => d.ExternalReference == reference);
    }
}
