using Assessment.Application.Abstractions;
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Persistence;

public class AssessmentDbContext(DbContextOptions<AssessmentDbContext> options)
    : DbContext(options), IAssessmentDbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssessmentDbContext).Assembly);
    }
}
