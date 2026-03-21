using Microsoft.OpenApi.Models;

namespace ProjectManagerAPI.Options;

/// <summary>
/// Extension methods for Swagger/OpenAPI configuration.
/// </summary>
public static class ApiSwaggerRegistration
{
    /// <summary>
    /// Adds Swagger generation with JWT Bearer authentication support.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with Swagger services added.</returns>
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token. Example: Bearer {token}, without write Bearer",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    bearerScheme,
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}