namespace Tessera.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tessera.Core.Interfaces;
using Tessera.Data.Repositories;

/// <summary>
/// Dependency injection extension methods for the Tessera.Data layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Tessera Data layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or connectionString is null.</exception>
    public static IServiceCollection AddTesseraData(
        this IServiceCollection services,
        string connectionString)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        // Register DbContext with SQLite
        services.AddDbContext<TesseraDbContext>(options =>
            options.UseSqlite(connectionString, sqliteOptions =>
                sqliteOptions.MigrationsAssembly("Tessera.Data")));

        // Register repositories as scoped
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IHabitCompletionRepository, HabitCompletionRepository>();

        return services;
    }
}