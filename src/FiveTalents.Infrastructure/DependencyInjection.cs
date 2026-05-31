using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Infrastructure.Identity;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Infrastructure.Services;
using FiveTalents.Infrastructure.Services.Email;
using FiveTalents.Infrastructure.Services.GoogleWorkspace;
using FiveTalents.Infrastructure.Services.Sms;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FiveTalents.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        string provider = configuration["DatabaseProvider"] ?? "Sqlite";
        string connectionString = ParseConnectionString(configuration.GetConnectionString("DefaultConnection") ?? "");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString,
                    npgsql => npgsql.MigrationsAssembly("FiveTalents.Infrastructure"));
            }
            else
            {
                options.UseSqlite(connectionString,
                    sqlite => sqlite.MigrationsAssembly("FiveTalents.Migrations.Sqlite"));
            }
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IOrganizationHierarchyService, OrganizationHierarchyService>();
        services.AddScoped<IUserLinkingService, UserLinkingService>();
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        if (environment.IsDevelopment())
        {
            services.AddScoped<IEmailService, DevEmailSender>();
        }
        else
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
        }

        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IGoogleWorkspaceService, GoogleWorkspaceService>();

        return services;
    }

    // Render provides DATABASE_URL as a postgres:// URI; Npgsql requires key=value format.
    private static string ParseConnectionString(string connectionString)
    {
        if (!connectionString.StartsWith("postgres://") && !connectionString.StartsWith("postgresql://"))
        {
            return connectionString;
        }

        Uri uri = new Uri(connectionString);
        string[] userInfo = uri.UserInfo.Split(':', 2);
        string username = Uri.UnescapeDataString(userInfo[0]);
        string password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        int port = uri.Port > 0 ? uri.Port : 5432;
        string database = uri.AbsolutePath.TrimStart('/');

        return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
