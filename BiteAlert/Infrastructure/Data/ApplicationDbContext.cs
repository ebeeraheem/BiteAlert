using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.NotificationModule;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.ReviewModule;
using BiteAlert.Modules.VendorModule;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Use TCP mapping strategy - all the types are mapped to individual tables
        // Each table contains columns for all properties on the corresponding entity type
        // including inherited properties!!
        builder.Entity<ApplicationUser>()
            .UseTpcMappingStrategy();

        builder.Entity<Vendor>()
            .ToTable("Vendors");

        builder.Entity<Customer>()
            .ToTable("Customers");

        // Ignore the base class mapping
        builder.Ignore<ApplicationUser>();

        // Configure column type for product price
        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // Configure entities to have sequential IDs on add
        builder.Entity<Product>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Entity<Review>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Entity<Notification>()
            .Property(n => n.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        // Configure relationships between entities
        builder.Entity<Vendor>()
            .HasMany(v => v.Products)
            .WithOne(p => p.Vendor)
            .HasForeignKey(p => p.VendorId)
            .IsRequired(false);
        
        builder.Entity<Customer>()
            .HasMany(c => c.Reviews)
            .WithOne(r => r.Customer)
            .HasForeignKey(r => r.CustomerId)
            .IsRequired();

        builder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .IsRequired();
    }
}
