using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BiteAlert.StartupConfigs;

public static class SwaggerConfig
{
    private const string _website = "https://ebeesule.netlify.app";

    public static void AddSwaggerConfigurations(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Add the API info and description
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "Bite Alert API",
                Description = "An API that alerts users of delicacy availability from vendors they are following.",
                Contact = new OpenApiContact()
                {
                    Name = "Ibrahim Suleiman",
                    Email = "ebeeraheem@gmail.com",
                    Url = new Uri(_website)
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
