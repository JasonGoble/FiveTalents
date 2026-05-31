using FiveTalents.Domain.Auth;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FiveTalents.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        await db.Database.MigrateAsync();

        // Seed roles
        foreach (string? role in new[]
        {
            AppRoles.SystemAdmin, AppRoles.TopLevelAdmin, AppRoles.MidLevelAdmin,
            AppRoles.LocalAdmin, AppRoles.Staff, AppRoles.Member, AppRoles.ReadOnly
        })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed default organization levels (Network → Region → Campus)
        if (!await db.OrganizationLevels.AnyAsync())
        {
            db.OrganizationLevels.AddRange(
                new OrganizationLevel { Level = 1, DisplayName = "Network", PluralDisplayName = "Networks", IsEnabled = true },
                new OrganizationLevel { Level = 2, DisplayName = "Region", PluralDisplayName = "Regions", IsEnabled = true },
                new OrganizationLevel { Level = 3, DisplayName = "Campus", PluralDisplayName = "Campuses", IsEnabled = true }
            );
            await db.SaveChangesAsync();
        }

        // Seed default organization
        if (!await db.Organizations.AnyAsync())
        {
            db.Organizations.Add(new Organization
            {
                Name = "My Church",
                Level = 3,
                IsActive = true,
                TimeZone = "America/Chicago"
            });
            await db.SaveChangesAsync();
        }

        // Seed default family roles
        if (!await db.FamilyRoles.AnyAsync())
        {
            var org = await db.Organizations.FirstAsync();
            db.FamilyRoles.AddRange(
                new FamilyRole { OrganizationId = org.Id, Name = "Adult", IsAdult = true, SortOrder = 10, IsActive = true },
                new FamilyRole { OrganizationId = org.Id, Name = "Child", IsAdult = false, SortOrder = 20, IsActive = true },
                new FamilyRole { OrganizationId = org.Id, Name = "Other", IsAdult = true, SortOrder = 99, IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // Seed default group types
        if (!await db.GroupTypes.AnyAsync())
        {
            var org = await db.Organizations.FirstAsync();
            db.GroupTypes.AddRange(
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Small Group", Color = "#4CAF50", IconName = "groups" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Ministry Team", Color = "#2196F3", IconName = "volunteer_activism" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Bible Study", Color = "#FF9800", IconName = "menu_book" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Prayer Group", Color = "#9C27B0", IconName = "self_improvement" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Youth", Color = "#F44336", IconName = "sports_soccer" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Children", Color = "#FF5722", IconName = "child_care" },
                new FiveTalents.Domain.Groups.GroupType { OrganizationId = org.Id, Name = "Leadership Team", Color = "#607D8B", IconName = "star" }
            );
            await db.SaveChangesAsync();
        }

        // Seed default admin user
        const string adminEmail = "admin@FiveTalents.local";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var org = await db.Organizations.FirstAsync();
            ApplicationUser admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                PrimaryOrganizationId = org.Id,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin1234!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AppRoles.SystemAdmin);
                logger.LogInformation("Default admin user created: {Email} / Admin1234!", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
