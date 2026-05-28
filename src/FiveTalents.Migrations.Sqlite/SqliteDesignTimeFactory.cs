using FiveTalents.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiveTalents.Migrations.Sqlite;

public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=FiveTalents.db",
                o => o.MigrationsAssembly("FiveTalents.Migrations.Sqlite"))
            .Options;
        return new ApplicationDbContext(options);
    }
}
