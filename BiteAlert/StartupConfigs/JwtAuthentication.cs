// Ignore Spelling: Jwt

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BiteAlert.StartupConfigs;

public static class JwtAuthentication
{
    public static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
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
    }
}
