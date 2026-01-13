using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Configurations;
using NivelAccesDate_CodeFirst.Models;

namespace NivelAccesDate_CodeFirst.Data;

public class ConversionsDbContext : DbContext
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
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost.;Database=conversions_code_first;User Id=mihai;Password=admin;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new ConversionConfiguration());
 }
}

