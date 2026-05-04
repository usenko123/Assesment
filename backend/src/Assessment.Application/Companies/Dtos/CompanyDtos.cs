using System.ComponentModel.DataAnnotations;
using Assessment.Application.Validation;
using Assessment.Application.Vacancies.Dtos;

namespace Assessment.Application.Companies.Dtos;

public record CompanyDto(int Id, string Name, string Address, IReadOnlyCollection<VacancyDto> Vacancies);

// DTOs duplication is intentional. 
public record CompanyCreateDto(
    [Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Name,
    [Required, NotWhitespace, StringLength(300, MinimumLength = 1)] string Address);

public record CompanyUpdateDto(
    [Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Name,
    [Required, NotWhitespace, StringLength(300, MinimumLength = 1)] string Address);
