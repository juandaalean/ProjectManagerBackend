using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

/// <summary>
/// Extension methods for registering application services.
/// </summary>
public static class ApplicationServiceRegistration
{
    /// <summary>
    /// Adds application services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with application services added.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}