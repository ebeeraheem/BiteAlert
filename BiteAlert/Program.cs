using BiteAlert.Exceptions;
using BiteAlert.Infrastructure.Data;
using BiteAlert.StartupConfigs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddApiVersionConfig();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMailerSendConfig(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerConfigurations();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomServices();

var app = builder.Build();

// Seed roles and default admin user
await app.Services.SeedRoles();
await app.Services.SeedAdminUser();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
