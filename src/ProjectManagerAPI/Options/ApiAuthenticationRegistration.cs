using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Security.Options;

namespace ProjectManagerAPI.Options;

/// <summary>
/// Extension methods for authentication and authorization configuration.
/// </summary>
public static class ApiAuthenticationRegistration
{
    /// <summary>
    /// Adds and configures JWT authentication and authorization for the API.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection with authentication services added.</returns>
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => options.SecretKey.Length >= 32, "Jwt:SecretKey must have at least 32 characters.")
            .Validate(options => options.AccessTokenExpirationMinutes > 0, "Jwt:AccessTokenExpirationMinutes must be greater than 0.")
            .ValidateOnStart();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddAuthorization();

        return services;
    }
}