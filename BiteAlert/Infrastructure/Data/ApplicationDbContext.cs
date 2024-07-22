using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.VendorModule;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vendor>()
            .Property(v => v.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");
    }
}
