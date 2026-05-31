using FiveTalents.Application.Organizations.DTOs;
using FiveTalents.Application.Organizations.Queries;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Organizations.Queries;

public class GetAllOrganizationsQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetAllOrganizationsQueryHandler _handler;

    public GetAllOrganizationsQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetAllOrganizationsQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_ReturnsOnlyActiveOrgs_WhenIncludeInactiveIsFalse()
    {
        _db.Organizations.AddRange(
            new Organization { Name = "Active Org", Level = 1, IsActive = true },
            new Organization { Name = "Inactive Org", Level = 1, IsActive = false });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllOrganizationsQuery(IncludeInactive: false), CancellationToken.None);

        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Active Org");
    }

    [Fact]
    public async Task Handle_ReturnsAllOrgs_WhenIncludeInactiveIsTrue()
    {
        _db.Organizations.AddRange(
            new Organization { Name = "Active Org", Level = 1, IsActive = true },
            new Organization { Name = "Inactive Org", Level = 1, IsActive = false });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllOrganizationsQuery(IncludeInactive: true), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ExcludesSoftDeletedOrgs()
    {
        _db.Organizations.AddRange(
            new Organization { Name = "Live Org", Level = 1, IsActive = true },
            new Organization { Name = "Deleted Org", Level = 1, IsActive = true, IsDeleted = true });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllOrganizationsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Live Org");
    }

    [Fact]
    public async Task Handle_IncludesMemberCount()
    {
        Organization org = new() { Name = "With Members", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
        _db.Members.AddRange(
            new Member { OrganizationId = org.Id, FirstName = "A", LastName = "B" },
            new Member { OrganizationId = org.Id, FirstName = "C", LastName = "D" });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllOrganizationsQuery(), CancellationToken.None);

        result.Single().MemberCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_OrdersByLevelThenName()
    {
        _db.Organizations.AddRange(
            new Organization { Name = "Zion Parish", Level = 2, IsActive = true },
            new Organization { Name = "Alpha Diocese", Level = 1, IsActive = true },
            new Organization { Name = "Beta Parish", Level = 2, IsActive = true });
        await _db.SaveChangesAsync();

        IList<OrganizationDto> result = (await _handler.Handle(new GetAllOrganizationsQuery(), CancellationToken.None)).ToList();

        result[0].Name.Should().Be("Alpha Diocese");
        result[1].Name.Should().Be("Beta Parish");
        result[2].Name.Should().Be("Zion Parish");
    }
}
