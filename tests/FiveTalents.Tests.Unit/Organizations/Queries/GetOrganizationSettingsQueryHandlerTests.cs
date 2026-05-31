using FiveTalents.Application.Organizations.Queries;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Organizations.Queries;

public class GetOrganizationSettingsQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetOrganizationSettingsQueryHandler _handler;

    public GetOrganizationSettingsQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetOrganizationSettingsQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingSettings_ReturnsPopulatedDto()
    {
        _db.OrganizationSettings.Add(new OrganizationSettings
        {
            OrganizationId = 1,
            PrimaryColor = "#336699",
            Currency = "EUR",
            FiscalYearStart = "09-01",
            EnableAttendanceTracking = true,
            EnableMemberPortal = false,
            GoogleWorkspaceDomain = "diocese.org",
            GoogleWorkspaceEnabled = true
        });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetOrganizationSettingsQuery(1), CancellationToken.None);

        result.PrimaryColor.Should().Be("#336699");
        result.Currency.Should().Be("EUR");
        result.FiscalYearStart.Should().Be("09-01");
        result.EnableAttendanceTracking.Should().BeTrue();
        result.EnableMemberPortal.Should().BeFalse();
        result.GoogleWorkspaceDomain.Should().Be("diocese.org");
        result.GoogleWorkspaceEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoSettings_ReturnsDefaults()
    {
        var result = await _handler.Handle(new GetOrganizationSettingsQuery(99), CancellationToken.None);

        result.Currency.Should().Be("USD");
        result.FiscalYearStart.Should().Be("01-01");
        result.EnableAttendanceTracking.Should().BeTrue();
        result.EnableOnlineGiving.Should().BeFalse();
        result.EnableMemberPortal.Should().BeFalse();
        result.GoogleWorkspaceEnabled.Should().BeFalse();
    }
}
