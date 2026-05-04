using Assessment.Application.Abstractions;
using Assessment.Application.Companies.Dtos;
using Assessment.Application.Companies.Mapping;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Application.Vacancies.Mapping;
using Assessment.Domain.Common;
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Companies.Services;

public class CompaniesService(IAssessmentDbContext dbContext) : ICompaniesService
{
    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync()
    {
        return await dbContext.Companies
            .AsNoTracking()
            .SelectCompanyDtos()
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetCompanyAsync(int id)
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .SelectCompanyDtos()
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesWithActiveVacanciesAsync()
    {
        return await dbContext.Companies
            .AsNoTracking()
            .SelectCompanyDtosWithActiveVacancies()
            .ToListAsync();
    }

    public async Task<Result<CompanyDto>> CreateCompanyAsync(CompanyCreateDto request)
    {
        var name = request.Name.Trim();
        var address = request.Address.Trim();

        var duplicate = await dbContext.Companies.AnyAsync(c => c.Name == name && c.Address == address);
        if (duplicate)
        {
            return Result<CompanyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Bedrijf met deze naam en adres bestaat al."));
        }

        var company = new Company
        {
            Name = name,
            Address = address
        };

        dbContext.Companies.Add(company);
        await dbContext.SaveChangesAsync();

        var response = new CompanyDto(company.Id, company.Name, company.Address, []);
        return Result<CompanyDto>.Ok(response);
    }

    public async Task<Result<CompanyDto>> UpdateCompanyAsync(int id, CompanyUpdateDto request)
    {
        var name = request.Name.Trim();
        var address = request.Address.Trim();

        var company = await dbContext.Companies
            .Include(c => c.Vacancies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
        {
            return Result<CompanyDto>.Fail(new AppError(AppErrorType.NotFound));
        }

        var duplicate = await dbContext.Companies.AnyAsync(c =>
            c.Id != id && c.Name == name && c.Address == address);
        if (duplicate)
        {
            return Result<CompanyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Bedrijf met deze naam en adres bestaat al."));
        }

        company.Name = name;
        company.Address = address;
        await dbContext.SaveChangesAsync();

        return Result<CompanyDto>.Ok(CompanyMappings.MapCompany(company));
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        var company = await dbContext.Companies.FindAsync(new object[] { id });
        if (company is null)
        {
            return false;
        }

        dbContext.Companies.Remove(company);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<VacancyDto>> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request)
    {
        var title = request.Title.Trim();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim();

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == companyId);
        if (!companyExists)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.NotFound,
                Title: "Niet gevonden",
                Detail: "Bedrijf niet gevonden."));
        }

        var duplicateVacancy = await dbContext.Vacancies.AnyAsync(v =>
            v.CompanyId == companyId && v.Title == title);
        if (duplicateVacancy)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Er bestaat al een vacature met deze titel bij dit bedrijf."));
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

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }
}
