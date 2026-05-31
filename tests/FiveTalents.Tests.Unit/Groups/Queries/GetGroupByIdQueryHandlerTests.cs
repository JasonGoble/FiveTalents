using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Groups.Queries;
using FiveTalents.Domain.Groups;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Groups.Queries;

public class GetGroupByIdQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetGroupByIdQueryHandler _handler;

    public GetGroupByIdQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetGroupByIdQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(Group group, GroupType groupType)> SeedGroupAsync(string name = "Test Group")
    {
        GroupType gt = new GroupType { OrganizationId = 1, Name = "Small Group", Color = "#336699", IconName = "people" };
        _db.GroupTypes.Add(gt);
        Group group = new Group
        {
            OrganizationId = 1,
            Name = name,
            GroupTypeId = gt.Id,
            Status = GroupStatus.Active,
            IsOpenToNewMembers = true,
            MaxCapacity = 10
        };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return (group, gt);
    }

    [Fact]
    public async Task Handle_WithExistingId_ReturnsDto()
    {
        var (group, gt) = await SeedGroupAsync("Evening Prayer");

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Evening Prayer");
        result.GroupTypeName.Should().Be("Small Group");
        result.GroupTypeColor.Should().Be("#336699");
        result.MaxCapacity.Should().Be(10);
        result.OrganizationId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new GetGroupByIdQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedGroup_ThrowsNotFoundException()
    {
        GroupType gt = new GroupType { OrganizationId = 1, Name = "Type" };
        _db.GroupTypes.Add(gt);
        Group group = new Group { OrganizationId = 1, Name = "Deleted", GroupTypeId = gt.Id, IsDeleted = true };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
