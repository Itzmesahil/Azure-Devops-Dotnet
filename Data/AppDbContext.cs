using DotNetDevOpsDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetDevOpsDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Visitor> Visitors => Set<Visitor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.ToTable("Visitors");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Name).HasMaxLength(100).IsRequired();
            entity.Property(v => v.VisitedAtUtc).IsRequired();
        });
    }
}
