using Microsoft.EntityFrameworkCore;
using ProfessionalAPI.Models;

namespace ProfessionalAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        mb.Entity<Product>(e =>
        {
            e.Property(p => p.Price).HasPrecision(18, 2); 
            e.HasOne(p => p.User)
             .WithMany(u => u.Products)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}