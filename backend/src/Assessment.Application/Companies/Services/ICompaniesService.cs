using Assessment.Application.Common;
using Assessment.Application.Companies.Dtos;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Common;

namespace Assessment.Application.Companies.Services;

public interface ICompaniesService
{
    Task<PagedResult<CompanyDto>> GetCompaniesAsync(CompanyQuery query, CancellationToken ct = default);
    Task<CompanyDto?> GetCompanyAsync(int id, CancellationToken ct = default);
    Task<Result<CompanyDto>> CreateCompanyAsync(CompanyCreateDto request, CancellationToken ct = default);
    Task<Result<CompanyDto>> UpdateCompanyAsync(int id, CompanyUpdateDto request, CancellationToken ct = default);
    Task<bool> DeleteCompanyAsync(int id, CancellationToken ct = default);
    Task<Result<VacancyDto>> CreateCompanyVacancyAsync(int companyId, CompanyVacancyCreateDto request, CancellationToken ct = default);
}
