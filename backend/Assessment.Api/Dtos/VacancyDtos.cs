namespace Assessment.Api.Dtos;

public record VacancyDto(int Id, string Title, string? Description, bool IsActive, int CompanyId);

public record VacancyCreateDto(string Title, string? Description, bool IsActive, int CompanyId);

public record VacancyUpdateDto(string Title, string? Description, bool IsActive, int CompanyId);

public record CompanyVacancyCreateDto(string Title, string? Description, bool IsActive);
