using Assessment.Api.Data;
using Assessment.Api.Dtos;
using Assessment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Services;

public enum CompanyWriteFailure
{
    None,
    ValidationFailed,
    CompanyNotFound
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
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Address))
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.ValidationFailed);
        }

        var company = new Company
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim()
        };

        dbContext.Companies.Add(company);
        await dbContext.SaveChangesAsync();

        var response = new CompanyDto(company.Id, company.Name, company.Address, []);
        return new CompanyWriteResult(response, null, CompanyWriteFailure.None);
    }

    public async Task<CompanyWriteResult> UpdateCompanyAsync(int id, CompanyUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Address))
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.ValidationFailed);
        }

        var company = await dbContext.Companies
            .Include(c => c.Vacancies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.CompanyNotFound);
        }

        company.Name = request.Name.Trim();
        company.Address = request.Address.Trim();
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
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.ValidationFailed);
        }

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == companyId);
        if (!companyExists)
        {
            return new CompanyWriteResult(null, null, CompanyWriteFailure.CompanyNotFound);
        }

        var vacancy = new Vacancy
        {
            CompanyId = companyId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
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
