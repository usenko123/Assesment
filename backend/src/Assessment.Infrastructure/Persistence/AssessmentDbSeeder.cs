using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Persistence;

public static class AssessmentDbSeeder
{
    public static async Task SeedAsync(AssessmentDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Companies.AnyAsync())
        {
            return;
        }

        var companies = new List<Company>
        {
            new()
            {
                Name = "JEX Demo B.V.",
                Address = "Rotterdam, Netherlands",
                Vacancies = new List<Vacancy>
                {
                    new()
                    {
                        Title = "Backoffice Medewerker",
                        Description = "Verwerk administratie en klantvragen.",
                        IsActive = true
                    },
                    new()
                    {
                        Title = "Recruitment Support",
                        Description = "Ondersteun het recruitment team.",
                        IsActive = false
                    }
                }
            },
            new()
            {
                Name = "TechNova",
                Address = "Amsterdam, Netherlands",
                Vacancies = new List<Vacancy>
                {
                    new()
                    {
                        Title = "Frontend Developer",
                        Description = "Bouw Angular interfaces.",
                        IsActive = true
                    }
                }
            },
            new()
            {
                Name = "OldCorp",
                Address = "Utrecht, Netherlands",
                Vacancies = new List<Vacancy>
                {
                    new()
                    {
                        Title = "Legacy Specialist",
                        Description = "Onderhoud oudere systemen.",
                        IsActive = false
                    }
                }
            }
        };

        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();
    }
}
