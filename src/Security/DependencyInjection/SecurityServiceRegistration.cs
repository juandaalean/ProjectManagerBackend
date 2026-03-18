using Application.Security;
using Microsoft.Extensions.DependencyInjection;
using Security.Services;

namespace Security.DependencyInjection;

/// <summary>
/// Extension methods for registering security services.
/// </summary>
public static class SecurityServiceRegistration
{
    /// <summary>
    /// Adds security services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with security services added.</returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
