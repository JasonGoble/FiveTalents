using FiveTalents.Domain.Families;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FiveTalents.Infrastructure.Persistence;

public static class DevDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var orgMap = await SeedOrganizationsAsync(db, logger);
        await SeedMembersAndFamiliesAsync(db, logger, orgMap);
    }

    private static async Task<Dictionary<string, int>> SeedOrganizationsAsync(
        ApplicationDbContext db, ILogger logger)
    {
        var orgMap = await db.Organizations.ToDictionaryAsync(o => o.Name, o => o.Id);

        if (orgMap.Values.Any(id => db.Organizations.Local.FirstOrDefault(o => o.Id == id)?.Level == 1)
            || await db.Organizations.AnyAsync(o => o.Level == 1))
        {
            return orgMap;
        }

        foreach (var spec in DevSeedData.Organizations())
        {
            int? parentId = spec.ParentName is not null ? orgMap[spec.ParentName] : null;
            Organization org = new Organization
            {
                Name = spec.Name,
                Level = spec.Level,
                ParentOrganizationId = parentId,
                IsActive = true,
                TimeZone = spec.TimeZone,
            };
            db.Organizations.Add(org);
            await db.SaveChangesAsync();
            orgMap[org.Name] = org.Id;
        }

        logger.LogInformation("Dev: seeded organization hierarchy ({Count} orgs)", DevSeedData.Organizations().Length);
        return orgMap;
    }

    private static async Task SeedMembersAndFamiliesAsync(
        ApplicationDbContext db, ILogger logger, Dictionary<string, int> orgMap)
    {
        if (await db.Members.AnyAsync())
        {
            return;
        }

        var adultRole = await db.FamilyRoles.FirstAsync(r => r.IsAdult);
        var childRole = await db.FamilyRoles.FirstAsync(r => !r.IsAdult);

        var memberSpecs = DevSeedData.Members();
        List<Member> domainMembers = memberSpecs.Select(s => new Member
        {
            FirstName = s.FirstName,
            LastName = s.LastName,
            Gender = s.Gender,
            MaritalStatus = s.MaritalStatus,
            Status = s.Status,
            DateOfBirth = s.DateOfBirth,
            JoinDate = s.JoinDate,
            OrganizationId = orgMap.TryGetValue(s.CampusName, out int orgId) ? orgId : orgMap.Values.First(),
        }).ToList();

        db.Members.AddRange(domainMembers);
        await db.SaveChangesAsync();

        Dictionary<(string FirstName, string LastName), Member> memberMap = domainMembers.ToDictionary(m => (m.FirstName, m.LastName));

        foreach (var spec in DevSeedData.Families())
        {
            if (!orgMap.TryGetValue(spec.CampusName, out int familyOrgId))
            {
                continue;
            }

            Family family = new Family { Name = spec.FamilyName, OrganizationId = familyOrgId };
            db.Families.Add(family);
            await db.SaveChangesAsync();

            foreach (var (first, last, isAdult) in spec.Lineup)
            {
                if (!memberMap.TryGetValue((first, last), out Member? member))
                {
                    continue;
                }

                db.FamilyMembers.Add(new FamilyMember
                {
                    FamilyId = family.Id,
                    MemberId = member.Id,
                    FamilyRoleId = isAdult ? adultRole.Id : childRole.Id,
                });
            }
            await db.SaveChangesAsync();
        }

        logger.LogInformation("Dev: seeded {Members} members, {Families} families",
            memberSpecs.Length, DevSeedData.Families().Length);
    }
}
