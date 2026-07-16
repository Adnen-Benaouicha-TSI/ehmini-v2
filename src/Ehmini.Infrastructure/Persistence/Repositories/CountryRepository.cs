using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Country?> GetByIsoCodeAsync(string isoCode)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.IsoCode == isoCode);
    }
}