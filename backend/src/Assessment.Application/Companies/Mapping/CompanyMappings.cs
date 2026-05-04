using Assessment.Application.Companies.Dtos;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Companies.Mapping;

internal static class CompanyMappings
{
    public static IQueryable<CompanyDto> SelectCompanyDtos(this IQueryable<Company> companies) =>
        companies.Select(c => new CompanyDto(
            c.Id,
            c.Name,
            c.Address,
            c.Vacancies
                .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                .ToList()));

    public static IQueryable<CompanyDto> SelectCompanyDtosWithActiveVacancies(this IQueryable<Company> companies) =>
        companies
            .Where(c => c.Vacancies.Any(v => v.IsActive))
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Where(v => v.IsActive)
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()));

    public static CompanyDto MapCompany(Company company) =>
        new(
            company.Id,
            company.Name,
            company.Address,
            company.Vacancies
                .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                .ToList());
}
