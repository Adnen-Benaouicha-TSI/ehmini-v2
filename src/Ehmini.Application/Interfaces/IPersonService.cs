using Ehmini.Application.DTOs.Auth;
using Ehmini.Application.DTOs.Person;

namespace Ehmini.Application.Interfaces;

public interface IPersonService
{
    Task<PersonDto> GetPersonAsync(CancellationToken cancellationToken);
    Task<UpdateUserInfoResponseDto> UpdateProfileAsync(UpdateUserInfoRequestDto dto, CancellationToken cancellationToken);
}
