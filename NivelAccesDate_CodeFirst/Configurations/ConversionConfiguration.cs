using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivelAccesDate_CodeFirst.Models;

namespace NivelAccesDate_CodeFirst.Configurations;

public class ConversionConfiguration : IEntityTypeConfiguration<Conversion>
{
    public void Configure(EntityTypeBuilder<Conversion> builder)
    {
        builder.ToTable("conversions");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.UserId)
            .HasColumnName("userId");

        builder.Property(e => e.ImageIdFrom)
            .HasColumnName("imageId_from");

        builder.Property(e => e.ImageIdTo)
            .HasColumnName("imageId_to");

        builder.Property(e => e.Datetime)
            .HasColumnName("datetime")
            .HasDefaultValueSql("GETDATE()");
    }
}

