namespace Assessment.Application.Companies.Dtos;

public sealed record CompanyQuery(
    string? Search = null,
    bool? HasActiveVacancies = null,
    int Page = 1,
    int PageSize = 20);
