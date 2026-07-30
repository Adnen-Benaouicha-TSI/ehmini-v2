using Ehmini.Application.DTOs;
using Ehmini.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ehmini.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessionController : ControllerBase
    {

        private readonly IProfessionService _professionService;


        public ProfessionController(IProfessionService professionService)
        {
            _professionService = professionService;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ProfessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfessions()
        {
            var professions = await _professionService.GetAllProfessionsAsync();
            return Ok(professions);
        }
    }
}
