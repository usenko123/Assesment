using Assessment.Application.Companies.Services;
using Assessment.Application.Vacancies.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICompaniesService, CompaniesService>();
        services.AddScoped<IVacanciesService, VacanciesService>();
        return services;
    }
}
