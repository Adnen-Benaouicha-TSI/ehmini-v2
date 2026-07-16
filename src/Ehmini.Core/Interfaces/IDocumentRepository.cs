using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ehmini.Core.Entities;

namespace Ehmini.Core.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId);
    Task<Document?> GetWithDetailsAsync(Guid id);
}
