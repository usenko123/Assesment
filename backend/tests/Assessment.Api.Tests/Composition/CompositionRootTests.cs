using Assessment.Application;
using Assessment.Application.Abstractions;
using Assessment.Application.Companies.Dtos;
using Assessment.Application.Companies.Services;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.Api.Tests.Composition;

public class CompositionRootTests
{
    [Fact]
    public async Task CompaniesService_GetCompanies_returns_empty_when_database_has_no_rows()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddDbContext<AssessmentDbContext>(options =>
            options.UseInMemoryDatabase($"Smoke_{Guid.NewGuid():N}"));
        services.AddScoped<IAssessmentDbContext>(sp =>
            sp.GetRequiredService<AssessmentDbContext>());

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var companiesService = scope.ServiceProvider.GetRequiredService<ICompaniesService>();

        var companies = await companiesService.GetCompaniesAsync(new CompanyQuery());

        Assert.Empty(companies.Items);
        Assert.Equal(0, companies.Total);
    }
}
