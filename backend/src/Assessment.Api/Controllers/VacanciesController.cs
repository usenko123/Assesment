using Assessment.Api.Extensions;
using Assessment.Application.Common;
using Assessment.Application.Vacancies.Dtos;
using Assessment.Application.Vacancies.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/vacancies")]
public class VacanciesController(IVacanciesService vacanciesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<VacancyDto>>> GetVacancies(
        [FromQuery] VacancyQuery query,
        CancellationToken ct)
    {
        var result = await vacanciesService.GetVacanciesAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<VacancyDto>> GetVacancy(int id, CancellationToken ct)
    {
        var vacancy = await vacanciesService.GetVacancyAsync(id, ct);
        return vacancy is null ? NotFound() : Ok(vacancy);
    }

    [HttpPost]
    public async Task<ActionResult<VacancyDto>> CreateVacancy(
        [FromBody] VacancyCreateDto request,
        CancellationToken ct)
    {
        var result = await vacanciesService.CreateVacancyAsync(request, ct);
        return this.ToActionResult(result, v => CreatedAtAction(nameof(GetVacancy), new { id = v.Id }, v));
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<VacancyDto>> UpdateVacancy(
        int id,
        [FromBody] VacancyUpdateDto request,
        CancellationToken ct)
    {
        var result = await vacanciesService.UpdateVacancyAsync(id, request, ct);
        return this.ToActionResult(result, v => Ok(v));
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteVacancy(int id, CancellationToken ct)
    {
        var deleted = await vacanciesService.DeleteVacancyAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
