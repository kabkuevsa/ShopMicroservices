using Microsoft.EntityFrameworkCore;
using OrderService.Models.Entities;

namespace OrderService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasPrecision(18, 2);
    }
}