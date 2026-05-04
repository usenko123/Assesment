using Assessment.Api.Dtos;
using Assessment.Api.Services;
using Microsoft.AspNetCore.Http;
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
        return result.Failure switch
        {
            CompanyWriteFailure.None => CreatedAtAction(nameof(GetCompany), new { id = result.Company!.Id }, result.Company),
            CompanyWriteFailure.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Bedrijf met deze naam en adres bestaat al."
            }),
            CompanyWriteFailure.ValidationFailed => Problem(
                title: "Ongeldige aanvraag",
                detail: "Validatie is mislukt.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Serverfout",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromBody] CompanyUpdateDto request)
    {
        var result = await companiesService.UpdateCompanyAsync(id, request);
        return result.Failure switch
        {
            CompanyWriteFailure.None => Ok(result.Company),
            CompanyWriteFailure.CompanyNotFound => NotFound(),
            CompanyWriteFailure.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Bedrijf met deze naam en adres bestaat al."
            }),
            CompanyWriteFailure.ValidationFailed => Problem(
                title: "Ongeldige aanvraag",
                detail: "Validatie is mislukt.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Serverfout",
                statusCode: StatusCodes.Status500InternalServerError)
        };
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
        return result.Failure switch
        {
            CompanyWriteFailure.None => CreatedAtAction(nameof(VacanciesController.GetVacancy), "Vacancies", new { id = result.Vacancy!.Id }, result.Vacancy),
            CompanyWriteFailure.CompanyNotFound => Problem(
                title: "Niet gevonden",
                detail: "Bedrijf niet gevonden.",
                statusCode: StatusCodes.Status404NotFound),
            CompanyWriteFailure.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Er bestaat al een vacature met deze titel bij dit bedrijf."
            }),
            CompanyWriteFailure.ValidationFailed => Problem(
                title: "Ongeldige aanvraag",
                detail: "Validatie is mislukt.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Serverfout",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
