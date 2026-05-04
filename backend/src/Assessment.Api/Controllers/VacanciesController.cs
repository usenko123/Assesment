using Assessment.Api.Extensions;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Application.Vacancies.Services;
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
        return result.ToActionResult(v => CreatedAtAction(nameof(GetVacancy), new { id = v.Id }, v));
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<VacancyDto>> UpdateVacancy(int id, [FromBody] VacancyUpdateDto request)
    {
        var result = await vacanciesService.UpdateVacancyAsync(id, request);
        return result.ToActionResult(v => Ok(v));
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
