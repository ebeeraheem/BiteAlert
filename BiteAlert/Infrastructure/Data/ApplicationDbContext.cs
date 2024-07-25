﻿using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.VendorModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace BiteAlert.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Generate sequential IDs for application user
        builder.Entity<ApplicationUser>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");
    }
}
