using FiveTalents.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
