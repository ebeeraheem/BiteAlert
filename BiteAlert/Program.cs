using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.Utilities;
using BiteAlert.Modules.VendorModule;
using BiteAlert.StartupConfigs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Get swagger config values from appsettings.json
var title = builder.Configuration.GetSection("SwaggerDoc:Title").Value;
var description = builder.Configuration.GetSection("SwaggerDoc:Description").Value;
var name = builder.Configuration.GetSection("SwaggerDoc:Contact:Name").Value;
var email = builder.Configuration.GetSection("SwaggerDoc:Contact:Email").Value;
var url = builder.Configuration.GetSection("SwaggerDoc:Contact:Url").Value;

builder.Services.AddSwaggerGen(options =>
{
    // Add the API info and description
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = title,
        Description = description,
        Contact = new OpenApiContact()
        {
            Name = name,
            Email = email,
            Url = new Uri(url!)
        }
    });

    // Add the authorize button in the SwaggerUI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your token here:",
    });

    // Add the lock icon to all endpoints in SwaggerUI
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            Array.Empty<string>()
        }
    });

    // Add XML comments in SwaggerUI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

// Configure database connection string
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<UserContextService>();

// Configure aspnet identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Retrieve JWT configuration values
var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience");
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");

// Validate JWT configuration values
if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience) ||
    string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT configuration values are missing.");
}

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                jwtKey))
        };
    });

// Require users of the app to be authenticated
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await Seeder.SeedRoles(services);
}

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
