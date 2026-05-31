using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Groups.Queries;
using FiveTalents.Domain.Auth;
using FiveTalents.Domain.Groups;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

using NSubstitute;

namespace FiveTalents.Tests.Unit.Groups.Queries;

public class GetAllGroupsQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly GetAllGroupsQueryHandler _handler;

    public GetAllGroupsQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _currentUser = Substitute.For<ICurrentUserService>();
        _currentUser.IsInRole(AppRoles.SystemAdmin).Returns(false);
        _handler = new GetAllGroupsQueryHandler(_db, _currentUser);
    }

    public void Dispose() => _db.Dispose();

    private async Task<GroupType> SeedGroupTypeAsync(int orgId = 1)
    {
        GroupType gt = new GroupType { OrganizationId = orgId, Name = "Small Group", Color = "#336699" };
        _db.GroupTypes.Add(gt);
        await _db.SaveChangesAsync();
        return gt;
    }

    [Fact]
    public async Task Handle_ReturnsOnlyGroupsInRequestedOrg()
    {
        var gt = await SeedGroupTypeAsync();
        _db.Groups.AddRange(
            new Group { OrganizationId = 1, Name = "Group A", GroupTypeId = gt.Id },
            new Group { OrganizationId = 2, Name = "Group B", GroupTypeId = gt.Id });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllGroupsQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Group A");
    }

    [Fact]
    public async Task Handle_ExcludesSoftDeletedGroups()
    {
        var gt = await SeedGroupTypeAsync();
        _db.Groups.AddRange(
            new Group { OrganizationId = 1, Name = "Live Group", GroupTypeId = gt.Id },
            new Group { OrganizationId = 1, Name = "Dead Group", GroupTypeId = gt.Id, IsDeleted = true });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllGroupsQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Live Group");
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsMatchingGroups()
    {
        var gt = await SeedGroupTypeAsync();
        _db.Groups.AddRange(
            new Group { OrganizationId = 1, Name = "Active", GroupTypeId = gt.Id, Status = GroupStatus.Active },
            new Group { OrganizationId = 1, Name = "Inactive", GroupTypeId = gt.Id, Status = GroupStatus.Inactive });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllGroupsQuery(1, Status: GroupStatus.Inactive), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Inactive");
    }

    [Fact]
    public async Task Handle_WithSearch_FiltersOnName()
    {
        var gt = await SeedGroupTypeAsync();
        _db.Groups.AddRange(
            new Group { OrganizationId = 1, Name = "Bible Study", GroupTypeId = gt.Id },
            new Group { OrganizationId = 1, Name = "Youth Group", GroupTypeId = gt.Id });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllGroupsQuery(1, Search: "Bible"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Bible Study");
    }

    [Fact]
    public async Task Handle_ReturnsGroupsOrderedByName()
    {
        var gt = await SeedGroupTypeAsync();
        _db.Groups.AddRange(
            new Group { OrganizationId = 1, Name = "Zeta Group", GroupTypeId = gt.Id },
            new Group { OrganizationId = 1, Name = "Alpha Group", GroupTypeId = gt.Id });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetAllGroupsQuery(1), CancellationToken.None);

        result[0].Name.Should().Be("Alpha Group");
        result[1].Name.Should().Be("Zeta Group");
    }
}
