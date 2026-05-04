using Assessment.Application.Abstractions;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AssessmentDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IAssessmentDbContext>(sp => sp.GetRequiredService<AssessmentDbContext>());
        return services;
    }
}
