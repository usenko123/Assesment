using Assessment.Api.Dtos;

namespace Assessment.Api.Services;

public interface ICompaniesService
{
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync();
    Task<CompanyDto?> GetCompanyAsync(int id);
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesWithActiveVacanciesAsync();
    Task<CompanyWriteResult> CreateCompanyAsync(CompanyCreateDto request);
    Task<CompanyWriteResult> UpdateCompanyAsync(int id, CompanyUpdateDto request);
    Task<bool> DeleteCompanyAsync(int id);
    Task<CompanyWriteResult> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request);
}
