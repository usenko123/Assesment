using Assessment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Data;

public class AssessmentDbContext(DbContextOptions<AssessmentDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Vacancies)
            .WithOne(v => v.Company)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Company>()
            .Property(c => c.Name)
            .IsRequired();

        modelBuilder.Entity<Company>()
            .Property(c => c.Address)
            .IsRequired();

        modelBuilder.Entity<Vacancy>()
            .Property(v => v.Title)
            .IsRequired();
    }
}
