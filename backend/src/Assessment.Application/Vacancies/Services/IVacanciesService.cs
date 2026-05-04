using Assessment.Application.Common;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Common;

namespace Assessment.Application.Vacancies.Services;

public interface IVacanciesService
{
    Task<PagedResult<VacancyDto>> GetVacanciesAsync(VacancyQuery query, CancellationToken ct = default);
    Task<VacancyDto?> GetVacancyAsync(int id, CancellationToken ct = default);
    Task<Result<VacancyDto>> CreateVacancyAsync(VacancyCreateDto request, CancellationToken ct = default);
    Task<Result<VacancyDto>> UpdateVacancyAsync(int id, VacancyUpdateDto request, CancellationToken ct = default);
    Task<bool> DeleteVacancyAsync(int id, CancellationToken ct = default);
}
