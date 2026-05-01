using Assessment.Api.Data;
using Assessment.Api.Dtos;
using Assessment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Services;

public enum VacancyWriteFailure
{
    None,
    ValidationFailed,
    VacancyNotFound,
    CompanyNotFound
}

public record VacancyWriteResult(VacancyDto? Vacancy, VacancyWriteFailure Failure);

public class VacanciesService(AssessmentDbContext dbContext) : IVacanciesService
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

    public async Task<VacancyWriteResult> CreateVacancyAsync(VacancyCreateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new VacancyWriteResult(null, VacancyWriteFailure.ValidationFailed);
        }

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId);
        if (!companyExists)
        {
            return new VacancyWriteResult(null, VacancyWriteFailure.CompanyNotFound);
        }

        var vacancy = new Vacancy
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = request.IsActive,
            CompanyId = request.CompanyId
        };

        dbContext.Vacancies.Add(vacancy);
        await dbContext.SaveChangesAsync();

        return new VacancyWriteResult(MapVacancy(vacancy), VacancyWriteFailure.None);
    }

    public async Task<VacancyWriteResult> UpdateVacancyAsync(int id, VacancyUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new VacancyWriteResult(null, VacancyWriteFailure.ValidationFailed);
        }

        var vacancy = await dbContext.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
        if (vacancy is null)
        {
            return new VacancyWriteResult(null, VacancyWriteFailure.VacancyNotFound);
        }

        var companyExists = await dbContext.Companies.AnyAsync(c => c.Id == request.CompanyId);
        if (!companyExists)
        {
            return new VacancyWriteResult(null, VacancyWriteFailure.CompanyNotFound);
        }

        vacancy.Title = request.Title.Trim();
        vacancy.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        vacancy.IsActive = request.IsActive;
        vacancy.CompanyId = request.CompanyId;

        await dbContext.SaveChangesAsync();

        return new VacancyWriteResult(MapVacancy(vacancy), VacancyWriteFailure.None);
    }

    public async Task<bool> DeleteVacancyAsync(int id)
    {
        var vacancy = await dbContext.Vacancies.FindAsync(id);
        if (vacancy is null)
        {
            return false;
        }

        dbContext.Vacancies.Remove(vacancy);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private static VacancyDto MapVacancy(Vacancy vacancy)
    {
        return new VacancyDto(vacancy.Id, vacancy.Title, vacancy.Description, vacancy.IsActive, vacancy.CompanyId);
    }
}
