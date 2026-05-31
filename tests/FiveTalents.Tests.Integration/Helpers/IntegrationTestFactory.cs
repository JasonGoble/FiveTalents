using FiveTalents.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiveTalents.Tests.Integration.Helpers;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public IntegrationTestFactory()
    {
        _connection = new SqliteConnection($"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared");
        _connection.Open();
    }

    public HttpClient CreateAuthenticatedClient(string? token = null)
    {
        HttpClient client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token ?? JwtTokenHelper.AdminToken());
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JwtSettings:Secret"] = JwtTokenHelper.TestSecret,
            ["JwtSettings:Issuer"] = JwtTokenHelper.TestIssuer,
            ["JwtSettings:Audience"] = JwtTokenHelper.TestAudience,
            ["JwtSettings:ExpiryHours"] = "1",
        }));

        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection,
                    sqlite => sqlite.MigrationsAssembly("FiveTalents.Migrations.Sqlite")));
        });
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
