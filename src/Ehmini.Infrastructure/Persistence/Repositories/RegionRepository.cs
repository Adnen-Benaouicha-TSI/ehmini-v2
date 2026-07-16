using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class RegionRepository : Repository<Region>, IRegionRepository
{
    public RegionRepository(ApplicationDbContext context) : base(context)
    {
    }
}