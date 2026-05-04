using Assessment.Application.Companies.Dtos;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Common;

namespace Assessment.Application.Companies.Services;

public interface ICompaniesService
{
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync();
    Task<CompanyDto?> GetCompanyAsync(int id);
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesWithActiveVacanciesAsync();
    Task<Result<CompanyDto>> CreateCompanyAsync(CompanyCreateDto request);
    Task<Result<CompanyDto>> UpdateCompanyAsync(int id, CompanyUpdateDto request);
    Task<bool> DeleteCompanyAsync(int id);
    Task<Result<VacancyDto>> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request);
}
