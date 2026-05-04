using Assessment.Api.Dtos;
using Assessment.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacanciesController(IVacanciesService vacanciesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VacancyDto>>> GetVacancies()
    {
        var vacancies = await vacanciesService.GetVacanciesAsync();
        return Ok(vacancies);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<VacancyDto>> GetVacancy(int id)
    {
        var vacancy = await vacanciesService.GetVacancyAsync(id);
        return vacancy is null ? NotFound() : Ok(vacancy);
    }

    [HttpPost]
    public async Task<ActionResult<VacancyDto>> CreateVacancy([FromBody] VacancyCreateDto request)
    {
        var result = await vacanciesService.CreateVacancyAsync(request);
        return result.Failure switch
        {
            VacancyWriteFailure.None => CreatedAtAction(nameof(GetVacancy), new { id = result.Vacancy!.Id }, result.Vacancy),
            VacancyWriteFailure.CompanyNotFound => Problem(
                title: "Ongeldig bedrijf",
                detail: "Bedrijf bestaat niet.",
                statusCode: StatusCodes.Status400BadRequest),
            VacancyWriteFailure.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Er bestaat al een vacature met deze titel bij dit bedrijf."
            }),
            VacancyWriteFailure.ValidationFailed => Problem(
                title: "Ongeldige aanvraag",
                detail: "Validatie is mislukt.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Serverfout",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<VacancyDto>> UpdateVacancy(int id, [FromBody] VacancyUpdateDto request)
    {
        var result = await vacanciesService.UpdateVacancyAsync(id, request);
        return result.Failure switch
        {
            VacancyWriteFailure.None => Ok(result.Vacancy),
            VacancyWriteFailure.VacancyNotFound => NotFound(),
            VacancyWriteFailure.CompanyNotFound => Problem(
                title: "Ongeldig bedrijf",
                detail: "Bedrijf bestaat niet.",
                statusCode: StatusCodes.Status400BadRequest),
            VacancyWriteFailure.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Er bestaat al een vacature met deze titel bij dit bedrijf."
            }),
            VacancyWriteFailure.ValidationFailed => Problem(
                title: "Ongeldige aanvraag",
                detail: "Validatie is mislukt.",
                statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(
                title: "Serverfout",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteVacancy(int id)
    {
        var deleted = await vacanciesService.DeleteVacancyAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
