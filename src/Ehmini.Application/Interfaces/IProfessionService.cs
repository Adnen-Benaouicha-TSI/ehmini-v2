
using Ehmini.Application.DTOs;

namespace Ehmini.Application.Interfaces
{
    public interface IProfessionService
    {
        Task<IEnumerable<ProfessionDto>> GetAllProfessionsAsync();
    }
}
