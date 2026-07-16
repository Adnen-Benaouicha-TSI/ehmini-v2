using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class ZoneRepository : Repository<Zone>, IZoneRepository
{
    public ZoneRepository(ApplicationDbContext context) : base(context)
    {
    }

}