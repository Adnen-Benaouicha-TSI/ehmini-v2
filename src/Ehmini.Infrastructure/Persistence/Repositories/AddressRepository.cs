using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ehmini.Infrastructure.Persistence.Repositories;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context)
    {
    }
}