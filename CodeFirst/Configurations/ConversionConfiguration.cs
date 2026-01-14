using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeFirst.Models;

namespace CodeFirst.Configurations;

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

        // Configure relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Conversions)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ImageFrom)
            .WithMany(i => i.ConversionsFrom)
            .HasForeignKey(c => c.ImageIdFrom)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ImageTo)
            .WithMany(i => i.ConversionsTo)
            .HasForeignKey(c => c.ImageIdTo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

