using Assessment.Application.Abstractions;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Application.Vacancies.Mapping;
using Assessment.Domain.Common;
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Vacancies.Services;

public class VacanciesService(IAssessmentDbContext dbContext) : IVacanciesService
{
    public async Task<IReadOnlyCollection<VacancyDto>> GetVacanciesAsync()
    {
        return await dbContext.Vacancies
            .AsNoTracking()
            .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
            .ToListAsync();
    }

    public async Task<VacancyDto?> GetVacancyAsync(int id)
    {
        return await dbContext.Vacancies
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new VacancyDto(v.Id, v.Title, v.Description, v.IsActive, v.CompanyId))
            .FirstOrDefaultAsync();
    }

    public async Task<Result<VacancyDto>> CreateVacancyAsync(VacancyCreateDto request)
    {
        var title = request.Title.Trim();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId);
        if (!companyExists)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.BadRequest,
                Title: "Ongeldig bedrijf",
                Detail: "Bedrijf bestaat niet."));
        }

        var duplicate = await dbContext.Vacancies.AnyAsync(v =>
            v.CompanyId == request.CompanyId && v.Title == title);
        if (duplicate)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Er bestaat al een vacature met deze titel bij dit bedrijf."));
        }

        var vacancy = new Vacancy
        {
            Title = title,
            Description = description,
            IsActive = request.IsActive,
            CompanyId = request.CompanyId
        };

        dbContext.Vacancies.Add(vacancy);
        await dbContext.SaveChangesAsync();

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }

    public async Task<Result<VacancyDto>> UpdateVacancyAsync(int id, VacancyUpdateDto request)
    {
        var title = request.Title.Trim();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        var vacancy = await dbContext.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
        if (vacancy is null)
        {
            return Result<VacancyDto>.Fail(new AppError(AppErrorType.NotFound));
        }

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId);
        if (!companyExists)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.BadRequest,
                Title: "Ongeldig bedrijf",
                Detail: "Bedrijf bestaat niet."));
        }

        var duplicate = await dbContext.Vacancies.AnyAsync(v =>
            v.Id != id && v.CompanyId == request.CompanyId && v.Title == title);
        if (duplicate)
        {
            return Result<VacancyDto>.Fail(new AppError(
                AppErrorType.Conflict,
                Title: "Er bestaat al een vacature met deze titel bij dit bedrijf."));
        }

        vacancy.Title = title;
        vacancy.Description = description;
        vacancy.IsActive = request.IsActive;
        vacancy.CompanyId = request.CompanyId;

        await dbContext.SaveChangesAsync();

        return Result<VacancyDto>.Ok(VacancyMappings.MapVacancy(vacancy));
    }

    public async Task<bool> DeleteVacancyAsync(int id)
    {
        var vacancy = await dbContext.Vacancies.FindAsync(new object[] { id });
        if (vacancy is null)
        {
            return false;
        }

        dbContext.Vacancies.Remove(vacancy);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
