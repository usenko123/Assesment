using Assessment.Application.Abstractions;
using Assessment.Application.Common;
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
    private static readonly AppError DuplicateCompanyError = new(
        AppErrorType.Conflict,
        Title: "Bedrijf met deze naam en adres bestaat al.");

    public async Task<PagedResult<CompanyDto>> GetCompaniesAsync(CompanyQuery query, CancellationToken ct = default)
    {
        var page = new PageQuery(query.Page, query.PageSize);

        var source = dbContext.Companies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(c =>
                EF.Functions.Like(c.Name, $"%{term}%") ||
                EF.Functions.Like(c.Address, $"%{term}%"));
        }

        if (query.HasActiveVacancies == true)
        {
            source = source.Where(c => c.Vacancies.Any(v => v.IsActive));
        }
        else if (query.HasActiveVacancies == false)
        {
            source = source.Where(c => !c.Vacancies.Any(v => v.IsActive));
        }

        var total = await source.CountAsync(ct);

        var items = await source
            .OrderBy(c => c.Id)
            .Skip(page.Skip)
            .Take(page.SafePageSize)
            .SelectCompanyDtos(query.HasActiveVacancies == true)
            .ToListAsync(ct);

        return new PagedResult<CompanyDto>(items, total, page.SafePage, page.SafePageSize);
    }

    public async Task<CompanyDto?> GetCompanyAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .SelectCompanyDtos(activeVacanciesOnly: false)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Result<CompanyDto>> CreateCompanyAsync(CompanyCreateDto request, CancellationToken ct = default)
    {
        var company = new Company
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim()
        };

        dbContext.Companies.Add(company);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<CompanyDto>.Fail(DuplicateCompanyError);
        }

        return Result<CompanyDto>.Ok(new CompanyDto(company.Id, company.Name, company.Address, []));
    }

    public async Task<Result<CompanyDto>> UpdateCompanyAsync(int id, CompanyUpdateDto request, CancellationToken ct = default)
    {
        var company = await dbContext.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (company is null)
        {
            return Result<CompanyDto>.Fail(new AppError(AppErrorType.NotFound));
        }

        company.Name = request.Name.Trim();
        company.Address = request.Address.Trim();

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<CompanyDto>.Fail(DuplicateCompanyError);
        }

        return Result<CompanyDto>.Ok(new CompanyDto(company.Id, company.Name, company.Address, []));
    }

    public async Task<bool> DeleteCompanyAsync(int id, CancellationToken ct = default)
    {
        var company = await dbContext.Companies.FindAsync([id], ct);
        if (company is null)
        {
            return false;
        }

        dbContext.Companies.Remove(company);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Result<VacancyDto>> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request, CancellationToken ct = default)
    {
        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == companyId, ct);
        if (!companyExists)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.NotFound,
                Title: "Niet gevonden",
                Detail: "Bedrijf niet gevonden."));
        }

        var vacancy = new Vacancy
        {
            CompanyId = companyId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Vacancies.Add(vacancy);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Er bestaat al een vacature met deze titel bij dit bedrijf."));
        }

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }
}
