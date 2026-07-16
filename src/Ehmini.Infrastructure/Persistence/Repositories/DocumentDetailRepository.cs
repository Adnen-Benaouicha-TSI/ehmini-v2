using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class DocumentDetailRepository : Repository<DocumentDetail>, IDocumentDetailRepository
{
    public DocumentDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

}
