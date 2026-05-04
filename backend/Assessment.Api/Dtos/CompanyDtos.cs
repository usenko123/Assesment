using System.ComponentModel.DataAnnotations;
using Assessment.Api.Validation;

namespace Assessment.Api.Dtos;

public record CompanyDto(int Id, string Name, string Address, IReadOnlyCollection<VacancyDto> Vacancies);

public record CompanyCreateDto(
    [property: Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Name,
    [property: Required, NotWhitespace, StringLength(300, MinimumLength = 1)] string Address);

public record CompanyUpdateDto(
    [property: Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Name,
    [property: Required, NotWhitespace, StringLength(300, MinimumLength = 1)] string Address);
