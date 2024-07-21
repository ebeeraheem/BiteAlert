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
}
