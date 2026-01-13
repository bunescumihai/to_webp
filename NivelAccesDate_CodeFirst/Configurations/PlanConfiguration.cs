using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivelAccesDate_CodeFirst.Models;

namespace NivelAccesDate_CodeFirst.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(e => e.Limit)
            .HasColumnName("limit");
        
        builder.Property(e => e.Price)
            .IsRequired()
            .HasColumnName("price");

        builder.HasData(
            new Plan
            {
                Id = 1,
                Name = "Free",
                Limit = 10,
                Price = 0
            },
            new Plan
            {
                Id = 2,
                Name = "Enterprise",
                Limit = 1000,
                Price = 10
            },
            new Plan
            {
                Id = 3,
                Name = "Premium",
                Limit = 1000,
                Price = 20
            }
        );
    }
}

