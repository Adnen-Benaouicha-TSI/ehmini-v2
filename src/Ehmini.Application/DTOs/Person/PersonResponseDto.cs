using Ehmini.Application.DTOs.Person;
namespace Ehmini.Application.DTOs.Person
{
    public record PersonResponseDto(
      int Id,
      string? Message,
      PersonDto Person
  );
}
