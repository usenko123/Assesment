using System.ComponentModel.DataAnnotations;
using Assessment.Application.Validation;

namespace Assessment.Application.Vacancies.Dtos;

public record VacancyDto(int Id, string Title, string? Description, bool IsActive, int CompanyId);

public record VacancyCreateDto(
    [Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [StringLength(2000)] string? Description,
    bool IsActive,
    [Range(1, int.MaxValue)] int CompanyId);

public record VacancyUpdateDto(
    [Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [StringLength(2000)] string? Description,
    bool IsActive,
    [Range(1, int.MaxValue)] int CompanyId);

public record CompanyVacancyCreateDto(
    [Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [StringLength(2000)] string? Description,
    bool IsActive);
