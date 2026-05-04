namespace Assessment.Application.Vacancies.Dtos;

public sealed record VacancyQuery(
    string? Search = null,
    int? CompanyId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20);
