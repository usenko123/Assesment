using Assessment.Api.Dtos;
using Assessment.Api.Services;
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

    [HttpGet("{id:int}")]
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
        if (result.Failure == CompanyWriteFailure.ValidationFailed)
        {
            return BadRequest("Naam en adres zijn verplicht.");
        }

        var response = result.Company!;
        return CreatedAtAction(nameof(GetCompany), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromBody] CompanyUpdateDto request)
    {
        var result = await companiesService.UpdateCompanyAsync(id, request);
        if (result.Failure == CompanyWriteFailure.ValidationFailed)
        {
            return BadRequest("Naam en adres zijn verplicht.");
        }

        if (result.Failure == CompanyWriteFailure.CompanyNotFound)
        {
            return NotFound();
        }

        return Ok(result.Company);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var deleted = await companiesService.DeleteCompanyAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{companyId:int}/vacancies")]
    public async Task<ActionResult<VacancyDto>> CreateCompanyVacancy(int companyId, [FromBody] CompanyVacancyCreateDto request)
    {
        var result = await companiesService.CreateCompanyVacancyAsync(companyId, request);
        if (result.Failure == CompanyWriteFailure.ValidationFailed)
        {
            return BadRequest("Titel is verplicht.");
        }

        if (result.Failure == CompanyWriteFailure.CompanyNotFound)
        {
            return NotFound("Bedrijf niet gevonden.");
        }

        var response = result.Vacancy!;
        return CreatedAtAction(nameof(VacanciesController.GetVacancy), "Vacancies", new { id = response.Id }, response);
    }
}
