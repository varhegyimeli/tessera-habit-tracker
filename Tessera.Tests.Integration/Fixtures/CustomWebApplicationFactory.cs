namespace Tessera.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Tessera.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext related registrations
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<TesseraDbContext>) ||
                d.ServiceType == typeof(TesseraDbContext) ||
                d.ServiceType == typeof(DbConnection)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Register the persistent in-memory connection
            services.AddSingleton<DbConnection>(_ => _connection);

            // Register DbContext using the persistent connection
            services.AddDbContext<TesseraDbContext>((sp, options) =>
            {
                var conn = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(conn);
            });
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://localhost/");
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TesseraDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}