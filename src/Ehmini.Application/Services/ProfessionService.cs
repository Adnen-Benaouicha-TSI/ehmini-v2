

using Ehmini.Application.DTOs;
using Ehmini.Application.Interfaces;
using Ehmini.Core.Interfaces;

namespace Ehmini.Application.Services
{
    public class ProfessionService : IProfessionService
    {


        private readonly IProfessionRepository _professionRepository;




        public ProfessionService(IProfessionRepository professionRepository)
        {
            _professionRepository = professionRepository;
        }

        public async Task<IEnumerable<ProfessionDto>> GetAllProfessionsAsync()
        {
            var professions = await _professionRepository.GetAllAsync();
            return professions.Select(p => new ProfessionDto(p.Id, p.Title));
        }



        }
}
