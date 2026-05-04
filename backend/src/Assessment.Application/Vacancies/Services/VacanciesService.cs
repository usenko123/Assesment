using Assessment.Application.Abstractions;
using Assessment.Application.Common;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Application.Vacancies.Mapping;
using Assessment.Domain.Common;
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Vacancies.Services;

public class VacanciesService(IAssessmentDbContext dbContext) : IVacanciesService
{
    private static readonly AppError DuplicateVacancyError = new(
        AppErrorType.Conflict,
        Title: "Er bestaat al een vacature met deze titel bij dit bedrijf.");

    private static readonly AppError InvalidCompanyError = new(
        AppErrorType.BadRequest,
        Title: "Ongeldig bedrijf",
        Detail: "Bedrijf bestaat niet.");

    public async Task<PagedResult<VacancyDto>> GetVacanciesAsync(VacancyQuery query, CancellationToken ct = default)
    {
        var page = new PageQuery(query.Page, query.PageSize);

        var source = dbContext.Vacancies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(v =>
                EF.Functions.Like(v.Title, $"%{term}%") ||
                (v.Description != null && EF.Functions.Like(v.Description, $"%{term}%")));
        }

        if (query.CompanyId is { } companyId)
        {
            source = source.Where(v => v.CompanyId == companyId);
        }

        if (query.IsActive is { } isActive)
        {
            source = source.Where(v => v.IsActive == isActive);
        }

        var total = await source.CountAsync(ct);

        var items = await source
            .OrderBy(v => v.Id)
            .Skip(page.Skip)
            .Take(page.SafePageSize)
            .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
            .ToListAsync(ct);

        return new PagedResult<VacancyDto>(items, total, page.SafePage, page.SafePageSize);
    }

    public async Task<VacancyDto?> GetVacancyAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Vacancies
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Result<VacancyDto>> CreateVacancyAsync(VacancyCreateDto request, CancellationToken ct = default)
    {
        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId, ct);
        if (!companyExists)
        {
            return Result<VacancyDto>.Fail(InvalidCompanyError);
        }

        var vacancy = new Vacancy
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = request.IsActive,
            CompanyId = request.CompanyId
        };

        dbContext.Vacancies.Add(vacancy);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<VacancyDto>.Fail(DuplicateVacancyError);
        }

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }

    public async Task<Result<VacancyDto>> UpdateVacancyAsync(int id, VacancyUpdateDto request, CancellationToken ct = default)
    {
        var vacancy = await dbContext.Vacancies.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vacancy is null)
        {
            return Result<VacancyDto>.Fail(new AppError(AppErrorType.NotFound));
        }

        if (vacancy.CompanyId != request.CompanyId)
        {
            var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId, ct);
            if (!companyExists)
            {
                return Result<VacancyDto>.Fail(InvalidCompanyError);
            }
        }

        vacancy.Title = request.Title.Trim();
        vacancy.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        vacancy.IsActive = request.IsActive;
        vacancy.CompanyId = request.CompanyId;

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<VacancyDto>.Fail(DuplicateVacancyError);
        }

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }

    public async Task<bool> DeleteVacancyAsync(int id, CancellationToken ct = default)
    {
        var vacancy = await dbContext.Vacancies.FindAsync([id], ct);
        if (vacancy is null)
        {
            return false;
        }

        dbContext.Vacancies.Remove(vacancy);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
