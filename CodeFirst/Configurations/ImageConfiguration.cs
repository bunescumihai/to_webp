using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeFirst.Models;

namespace CodeFirst.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("images");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Md5)
            .IsRequired()
            .HasMaxLength(32)
            .HasColumnName("md5");

        builder.Property(e => e.Path)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("path");

        builder.Property(e => e.Size)
            .HasColumnName("size");

        builder.Property(e => e.Format)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("format");

        builder.HasMany(e => e.ConversionsFrom)
            .WithOne(c => c.ImageFrom)
            .HasForeignKey(c => c.ImageIdFrom)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.ConversionsTo)
            .WithOne(c => c.ImageTo)
            .HasForeignKey(c => c.ImageIdTo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

