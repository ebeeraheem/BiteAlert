using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.VendorModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVendorService, VendorService>();


// What do I need to do in order to use UserManager?
builder.Services.AddIdentity<Vendor, IdentityRole>()
    //.AddEntityFrameworkStores<ApplicationDbContext>()
    //.AddUserManager<UserManager<Vendor>>()
    .AddUserStore<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
