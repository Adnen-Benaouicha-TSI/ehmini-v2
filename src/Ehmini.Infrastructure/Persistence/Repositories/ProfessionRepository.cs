using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class ProfessionRepository : Repository<Profession>, IProfessionRepository
{
    public ProfessionRepository(ApplicationDbContext context) : base(context)
    {
    }

}