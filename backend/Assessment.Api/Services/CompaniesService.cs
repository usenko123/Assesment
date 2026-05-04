using Assessment.Api.Data;
using Assessment.Api.Dtos;
using Assessment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Services;

public enum CompanyWriteFailure
{
    None,
    ValidationFailed,
    CompanyNotFound,
    Conflict
}

public record CompanyWriteResult(CompanyDto? Company, VacancyDto? Vacancy, CompanyWriteFailure Failure);

public class CompaniesService(AssessmentDbContext dbContext) : ICompaniesService
{
    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync()
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()))
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetCompanyAsync(int id)
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()))
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesWithActiveVacanciesAsync()
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Vacancies.Any(v => v.IsActive))
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Address,
                c.Vacancies
                    .Where(v => v.IsActive)
                    .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                    .ToList()))
            .ToListAsync();
    }

    public async Task<CompanyWriteResult> CreateCompanyAsync(CompanyCreateDto request)
    {
        var name = request.Name.Trim();
        var address = request.Address.Trim();

        var duplicate = await dbContext.Companies.AnyAsync(c => c.Name == name && c.Address == address);
        if (duplicate)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.Conflict);
        }

        var company = new Company
        {
            Name = name,
            Address = address
        };

        dbContext.Companies.Add(company);
        await dbContext.SaveChangesAsync();

        var response = new CompanyDto(company.Id, company.Name, company.Address, []);
        return new CompanyWriteResult(response, null, CompanyWriteFailure.None);
    }

    public async Task<CompanyWriteResult> UpdateCompanyAsync(int id, CompanyUpdateDto request)
    {
        var name = request.Name.Trim();
        var address = request.Address.Trim();

        var company = await dbContext.Companies
            .Include(c => c.Vacancies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.CompanyNotFound);
        }

        var duplicate = await dbContext.Companies.AnyAsync(c =>
            c.Id != id && c.Name == name && c.Address == address);
        if (duplicate)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.Conflict);
        }

        company.Name = name;
        company.Address = address;
        await dbContext.SaveChangesAsync();

        return new CompanyWriteResult(MapCompany(company), null, CompanyWriteFailure.None);
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        var company = await dbContext.Companies.FindAsync(id);
        if (company is null)
        {
            return false;
        }

        dbContext.Companies.Remove(company);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<CompanyWriteResult> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request)
    {
        var title = request.Title.Trim();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim();

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == companyId);
        if (!companyExists)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.CompanyNotFound);
        }

        var duplicateVacancy = await dbContext.Vacancies.AnyAsync(v =>
            v.CompanyId == companyId && v.Title == title);
        if (duplicateVacancy)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.Conflict);
        }

        var vacancy = new Vacancy
        {
            CompanyId = companyId,
            Title = title,
            Description = description,
            IsActive = request.IsActive
        };

        dbContext.Vacancies.Add(vacancy);
        await dbContext.SaveChangesAsync();

        var response = new VacancyDto(vacancy.Id, vacancy.Title, vacancy.Description, vacancy.IsActive, vacancy.CompanyId);
        return new CompanyWriteResult(null, response, CompanyWriteFailure.None);
    }

    private static CompanyDto MapCompany(Company company)
    {
        return new CompanyDto(
            company.Id,
            company.Name,
            company.Address,
            company.Vacancies
                .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
                .ToList());
    }
}
