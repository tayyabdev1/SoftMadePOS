using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Models;
namespace Shop;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleDetails> SaleDetails { get; set; }

    public DbSet<CustomerPayment> CustomerPayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique BarCode Rule
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode)
            .IsUnique();

        // Cascade Delete
        modelBuilder.Entity<SaleDetails>()
            .HasOne(sd => sd.Sale)
            .WithMany(s => s.SaleDetails)
            .HasForeignKey(sd => sd.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
