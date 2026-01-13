using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivelAccesDate_CodeFirst.Models;

namespace NivelAccesDate_CodeFirst.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("password");

        builder.Property(e => e.PlanId)
            .HasColumnName("planId");

        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("role");

        builder.HasOne(e => e.Plan)
            .WithMany(p => p.Users)
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_users_plans");

        builder.HasMany(e => e.Conversions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

