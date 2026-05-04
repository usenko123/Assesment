using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assessment.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasMany(c => c.Vacancies)
            .WithOne(v => v.Company)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Address).IsRequired();

        builder
            .HasIndex(c => new { c.Name, c.Address })
            .IsUnique();
    }
}
