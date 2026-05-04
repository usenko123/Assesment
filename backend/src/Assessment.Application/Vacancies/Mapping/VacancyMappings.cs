using Assessment.Application.Vacancies.Dtos;
using Assessment.Domain.Entities;

namespace Assessment.Application.Vacancies.Mapping;

internal static class VacancyMappings
{
    public static VacancyDto MapVacancy(Vacancy vacancy) =>
        new(vacancy.Id, vacancy.Title, vacancy.Description, vacancy.IsActive, vacancy.CompanyId);
}
