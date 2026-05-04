using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Abstractions;

public interface IAssessmentDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<Vacancy> Vacancies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
