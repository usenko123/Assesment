using Assessment.Application.Companies.Dtos;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Entities;

namespace Assessment.Application.Companies.Mapping;

internal static class CompanyMappings
{
    public static IQueryable<CompanyDto> SelectCompanyDtos(this IQueryable<Company> companies, bool activeVacanciesOnly) =>
        activeVacanciesOnly
            ? companies.Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Where(v => v.IsActive)
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()))
            : companies.Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()));
}
