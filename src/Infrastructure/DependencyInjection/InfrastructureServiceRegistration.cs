using Domain.Abstractions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection with infrastructure services added.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<ProjectManagerContext>(options =>
            options.UseNpgsql(NormalizeConnectionString(connectionString)));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProjectRepository, UserProjectRepository>();
        services.AddScoped<ISprintRepository, SprintRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static string NormalizeConnectionString(string connectionString)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, "postgresql", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(uri.Scheme, "postgres", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = uri.AbsolutePath.Trim('/'),
            Username = string.Empty,
            Password = string.Empty
        };

        if (!string.IsNullOrWhiteSpace(uri.UserInfo))
        {
            var userInfoParts = uri.UserInfo.Split(':', 2);
            builder.Username = Uri.UnescapeDataString(userInfoParts[0]);

            if (userInfoParts.Length > 1)
            {
                builder.Password = Uri.UnescapeDataString(userInfoParts[1]);
            }
        }

        foreach (var (key, value) in ParseQueryString(uri.Query))
        {
            if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase))
            {
                builder["Ssl Mode"] = value;
                continue;
            }

            if (key.Equals("channel_binding", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("channelbinding", StringComparison.OrdinalIgnoreCase))
            {
                builder["Channel Binding"] = value;
                continue;
            }

            builder[key.Replace('_', ' ')] = value;
        }

        return builder.ConnectionString;
    }

    private static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            yield break;
        }

        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0]);
            var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;

            yield return new KeyValuePair<string, string>(key, value);
        }
    }
}