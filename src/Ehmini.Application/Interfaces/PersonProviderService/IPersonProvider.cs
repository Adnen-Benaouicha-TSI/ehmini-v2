using Ehmini.Application.DTOs.Contracts;
using Ehmini.Application.DTOs.Person;
using Ehmini.Application.DTOs.QuotationList;
using Ehmini.Application.DTOs.Quotes;
using Ehmini.Core.Enum;


namespace Ehmini.Application.Interfaces.PersonProviderService
{
    public interface IPersonProvider
    {
        ProviderType Provider { get; }
        Task<PersonDto> GetPersonAsync(CancellationToken cancellationToken);
    }
}
