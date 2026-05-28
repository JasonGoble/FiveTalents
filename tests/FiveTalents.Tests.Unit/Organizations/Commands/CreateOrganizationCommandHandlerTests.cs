using FiveTalents.Application.Organizations.Commands;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Organizations.Commands;

public class CreateOrganizationCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new CreateOrganizationCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithMinimalCommand_ReturnsPositiveId()
    {
        var command = new CreateOrganizationCommand { Name = "Diocese of Test", Level = 1 };

        int id = await _handler.Handle(command, CancellationToken.None);

        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithValidCommand_StoresOrganizationWithCorrectProperties()
    {
        var command = new CreateOrganizationCommand
        {
            Name = "St. Paul's Parish",
            Level = 2,
            ParentOrganizationId = null,
            Description = "A test parish",
            Email = "info@stpauls.test",
            City = "Springfield",
            State = "IL",
            Country = "US"
        };

        int id = await _handler.Handle(command, CancellationToken.None);

        var org = await _db.Organizations.FindAsync(id);
        org.Should().NotBeNull();
        org!.Name.Should().Be("St. Paul's Parish");
        org.Level.Should().Be(2);
        org.Email.Should().Be("info@stpauls.test");
        org.City.Should().Be("Springfield");
        org.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CreatesDefaultOrganizationSettings()
    {
        var command = new CreateOrganizationCommand { Name = "Test Org", Level = 1 };

        int id = await _handler.Handle(command, CancellationToken.None);

        var settings = await _db.OrganizationSettings.FirstOrDefaultAsync(s => s.OrganizationId == id);
        settings.Should().NotBeNull();
        settings!.Currency.Should().Be("USD");
        settings.EnableAttendanceTracking.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MultipleOrgs_GetUniqueIds()
    {
        var cmd1 = new CreateOrganizationCommand { Name = "Org One", Level = 1 };
        var cmd2 = new CreateOrganizationCommand { Name = "Org Two", Level = 1 };

        int id1 = await _handler.Handle(cmd1, CancellationToken.None);
        int id2 = await _handler.Handle(cmd2, CancellationToken.None);

        id1.Should().NotBe(id2);
    }
}
