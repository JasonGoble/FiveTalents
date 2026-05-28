using FiveTalents.Application.Organizations.Commands;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Organizations.Commands;

public class UpdateOrganizationSettingsCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly UpdateOrganizationSettingsCommandHandler _handler;

    public UpdateOrganizationSettingsCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new UpdateOrganizationSettingsCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingSettings_UpdatesAllProperties()
    {
        _db.OrganizationSettings.Add(new OrganizationSettings
        {
            OrganizationId = 1, Currency = "USD", FiscalYearStart = "01-01"
        });
        await _db.SaveChangesAsync();

        var command = new UpdateOrganizationSettingsCommand
        {
            OrganizationId = 1,
            PrimaryColor = "#FF0000",
            Currency = "EUR",
            FiscalYearStart = "09-01",
            EnableAttendanceTracking = true,
            EnableMemberPortal = true,
            GoogleWorkspaceDomain = "parish.org",
            GoogleWorkspaceEnabled = true
        };

        await _handler.Handle(command, CancellationToken.None);

        var settings = await _db.OrganizationSettings.FirstAsync(s => s.OrganizationId == 1);
        settings.PrimaryColor.Should().Be("#FF0000");
        settings.Currency.Should().Be("EUR");
        settings.FiscalYearStart.Should().Be("09-01");
        settings.EnableAttendanceTracking.Should().BeTrue();
        settings.EnableMemberPortal.Should().BeTrue();
        settings.GoogleWorkspaceDomain.Should().Be("parish.org");
        settings.GoogleWorkspaceEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoExistingSettings_CreatesNewSettings()
    {
        var command = new UpdateOrganizationSettingsCommand
        {
            OrganizationId = 42,
            Currency = "GBP",
            FiscalYearStart = "04-01"
        };

        await _handler.Handle(command, CancellationToken.None);

        var settings = await _db.OrganizationSettings.FirstOrDefaultAsync(s => s.OrganizationId == 42);
        settings.Should().NotBeNull();
        settings!.Currency.Should().Be("GBP");
        settings.FiscalYearStart.Should().Be("04-01");
    }
}
