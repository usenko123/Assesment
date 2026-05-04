using Assessment.Api.Extensions;
using Assessment.Application.Common;
using Assessment.Application.Companies.Dtos;
using Assessment.Application.Companies.Services;
using Assessment.Application.Vacancies.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController(ICompaniesService companiesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CompanyDto>>> GetCompanies(
        [FromQuery] CompanyQuery query,
        CancellationToken ct)
    {
        var result = await companiesService.GetCompaniesAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id, CancellationToken ct)
    {
        var company = await companiesService.GetCompanyAsync(id, ct);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(
        [FromBody] CompanyCreateDto request,
        CancellationToken ct)
    {
        var result = await companiesService.CreateCompanyAsync(request, ct);
        return this.ToActionResult(result, c => CreatedAtAction(nameof(GetCompany), new { id = c.Id }, c));
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(
        int id,
        [FromBody] CompanyUpdateDto request,
        CancellationToken ct)
    {
        var result = await companiesService.UpdateCompanyAsync(id, request, ct);
        return this.ToActionResult(result, c => Ok(c));
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteCompany(int id, CancellationToken ct)
    {
        var deleted = await companiesService.DeleteCompanyAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{companyId:int:min(1)}/vacancies")]
    public async Task<ActionResult<VacancyDto>> CreateCompanyVacancy(
        int companyId,
        [FromBody] CompanyVacancyCreateDto request,
        CancellationToken ct)
    {
        var result = await companiesService.CreateCompanyVacancyAsync(companyId, request, ct);
        return this.ToActionResult(result, v =>
            CreatedAtAction(nameof(VacanciesController.GetVacancy), "Vacancies", new { id = v.Id }, v));
    }
}
