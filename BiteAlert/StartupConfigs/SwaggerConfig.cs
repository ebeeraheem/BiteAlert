using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BiteAlert.StartupConfigs;

public static class SwaggerConfig
{
    public static void AddSwaggerConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        // Get swagger config values from appsettings.json
        var title = configuration.GetSection("SwaggerDoc:Title").Value;
        var description = configuration.GetSection("SwaggerDoc:Description").Value;
        var name = configuration.GetSection("SwaggerDoc:Contact:Name").Value;
        var email = configuration.GetSection("SwaggerDoc:Contact:Email").Value;
        var url = configuration.GetSection("SwaggerDoc:Contact:Url").Value;

        services.AddSwaggerGen(options =>
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
    }
}
