namespace Assessment.Api.Dtos;

public record CompanyDto(int Id, string Name, string Address, IReadOnlyCollection<VacancyDto> Vacancies);

public record CompanyCreateDto(string Name, string Address);

public record CompanyUpdateDto(string Name, string Address);
