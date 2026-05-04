using Assessment.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Assessment.Infrastructure;

public static class DevelopmentSeedingExtensions
{
    public static async Task UseDevelopmentSeedingAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AssessmentDbContext>();
        await AssessmentDbSeeder.SeedAsync(dbContext);
    }
}
