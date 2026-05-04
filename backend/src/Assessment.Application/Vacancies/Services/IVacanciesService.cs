using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Common;

namespace Assessment.Application.Vacancies.Services;

public interface IVacanciesService
{
    Task<IReadOnlyCollection<VacancyDto>> GetVacanciesAsync();
    Task<VacancyDto?> GetVacancyAsync(int id);
    Task<Result<VacancyDto>> CreateVacancyAsync(VacancyCreateDto request);
    Task<Result<VacancyDto>> UpdateVacancyAsync(int id, VacancyUpdateDto request);
    Task<bool> DeleteVacancyAsync(int id);
}
