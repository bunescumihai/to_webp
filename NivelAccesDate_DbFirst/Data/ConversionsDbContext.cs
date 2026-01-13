﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NivelAccesDate_DbFirst.Models;

namespace NivelAccesDate_DbFirst.Data;

public partial class ConversionsDbContext : DbContext
{
    public ConversionsDbContext()
    {
    }

    public ConversionsDbContext(DbContextOptions<ConversionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conversion> Conversions { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string will be configured in the main application
        if (!optionsBuilder.IsConfigured)
        {
            // Optional: Add a default connection string for development/testing
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__conversi__3213E83F54B2F782");

            entity.ToTable("conversions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datetime)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("datetime");
            entity.Property(e => e.ImageIdFrom).HasColumnName("imageId_from");
            entity.Property(e => e.ImageIdTo).HasColumnName("imageId_to");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ImageIdFromNavigation).WithMany(p => p.ConversionImageIdFromNavigations)
                .HasForeignKey(d => d.ImageIdFrom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_conversions_image_from");

            entity.HasOne(d => d.ImageIdToNavigation).WithMany(p => p.ConversionImageIdToNavigations)
                .HasForeignKey(d => d.ImageIdTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_conversions_image_to");

            entity.HasOne(d => d.User).WithMany(p => p.Conversions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_conversions_users");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__images__3213E83F079D9756");

            entity.ToTable("images");

            entity.HasIndex(e => e.Md5, "UQ__images__DF502978BB11ED33").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Format)
                .HasMaxLength(10)
                .HasColumnName("format");
            entity.Property(e => e.Md5)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("md5");
            entity.Property(e => e.Path)
                .HasMaxLength(500)
                .HasColumnName("path");
            entity.Property(e => e.Size).HasColumnName("size");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__plans__3213E83F89E9388A");

            entity.ToTable("plans");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Limit).HasColumnName("limit");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FD14CF162");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61644F26E9B2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PlanId).HasColumnName("planId");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Plan).WithMany(p => p.Users)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_plans");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
