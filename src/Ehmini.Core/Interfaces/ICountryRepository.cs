using Ehmini.Core.Entities;

namespace Ehmini.Core.Interfaces;

public interface ICountryRepository : IRepository<Country>
{
    Task<Country?> GetByIsoCodeAsync(string isoCode);
}