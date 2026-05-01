using Assessment.Api.Dtos;

namespace Assessment.Api.Services;

public interface IVacanciesService
{
    Task<IReadOnlyCollection<VacancyDto>> GetVacanciesAsync();
    Task<VacancyDto?> GetVacancyAsync(int id);
    Task<VacancyWriteResult> CreateVacancyAsync(VacancyCreateDto request);
    Task<VacancyWriteResult> UpdateVacancyAsync(int id, VacancyUpdateDto request);
    Task<bool> DeleteVacancyAsync(int id);
}
