using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class LocalityRepository : Repository<Locality>, ILocalityRepository
{
    public LocalityRepository(ApplicationDbContext context) : base(context)
    {
    }
}