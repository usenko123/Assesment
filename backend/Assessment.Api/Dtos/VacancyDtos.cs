using System.ComponentModel.DataAnnotations;
using Assessment.Api.Validation;

namespace Assessment.Api.Dtos;

public record VacancyDto(int Id, string Title, string? Description, bool IsActive, int CompanyId);

public record VacancyCreateDto(
    [property: Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [property: StringLength(2000)] string? Description,
    bool IsActive,
    [property: Range(1, int.MaxValue)] int CompanyId);

public record VacancyUpdateDto(
    [property: Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [property: StringLength(2000)] string? Description,
    bool IsActive,
    [property: Range(1, int.MaxValue)] int CompanyId);

public record CompanyVacancyCreateDto(
    [property: Required, NotWhitespace, StringLength(200, MinimumLength = 1)] string Title,
    [property: StringLength(2000)] string? Description,
    bool IsActive);
