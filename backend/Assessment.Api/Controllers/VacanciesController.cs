using Assessment.Api.Dtos;
using Assessment.Api.Services;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VacancyDto>> GetVacancy(int id)
    {
        var vacancy = await vacanciesService.GetVacancyAsync(id);
        return vacancy is null ? NotFound() : Ok(vacancy);
    }

    [HttpPost]
    public async Task<ActionResult<VacancyDto>> CreateVacancy([FromBody] VacancyCreateDto request)
    {
        var result = await vacanciesService.CreateVacancyAsync(request);
        if (result.Failure == VacancyWriteFailure.ValidationFailed)
        {
            return BadRequest("Titel is verplicht.");
        }

        if (result.Failure == VacancyWriteFailure.CompanyNotFound)
        {
            return BadRequest("Bedrijf bestaat niet.");
        }

        var response = result.Vacancy!;
        return CreatedAtAction(nameof(GetVacancy), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VacancyDto>> UpdateVacancy(int id, [FromBody] VacancyUpdateDto request)
    {
        var result = await vacanciesService.UpdateVacancyAsync(id, request);
        if (result.Failure == VacancyWriteFailure.ValidationFailed)
        {
            return BadRequest("Titel is verplicht.");
        }

        if (result.Failure == VacancyWriteFailure.VacancyNotFound)
        {
            return NotFound();
        }

        if (result.Failure == VacancyWriteFailure.CompanyNotFound)
        {
            return BadRequest("Bedrijf bestaat niet.");
        }

        return Ok(result.Vacancy);
    }

    [HttpDelete("{id:int}")]
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
