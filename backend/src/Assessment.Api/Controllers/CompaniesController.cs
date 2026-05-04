using Assessment.Api.Extensions;
using Assessment.Application.Companies.Dtos;
using Assessment.Application.Companies.Services;
using Assessment.Application.Vacancies.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(ICompaniesService companiesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
    {
        var companies = await companiesService.GetCompaniesAsync();
        return Ok(companies);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id)
    {
        var company = await companiesService.GetCompanyAsync(id);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpGet("with-active-vacancies")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompaniesWithActiveVacancies()
    {
        var companies = await companiesService.GetCompaniesWithActiveVacanciesAsync();
        return Ok(companies);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CompanyCreateDto request)
    {
        var result = await companiesService.CreateCompanyAsync(request);
        return result.ToActionResult(c => CreatedAtAction(nameof(GetCompany), new { id = c.Id }, c));
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromBody] CompanyUpdateDto request)
    {
        var result = await companiesService.UpdateCompanyAsync(id, request);
        return result.ToActionResult(c => Ok(c));
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var deleted = await companiesService.DeleteCompanyAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{companyId:int:min(1)}/vacancies")]
    public async Task<ActionResult<VacancyDto>> CreateCompanyVacancy(int companyId, [FromBody] CompanyVacancyCreateDto request)
    {
        var result = await companiesService.CreateCompanyVacancyAsync(companyId, request);
        return result.ToActionResult(v =>
            CreatedAtAction(nameof(VacanciesController.GetVacancy), "Vacancies", new { id = v.Id }, v));
    }
}
